using AutoMapper;
using LeaveCore.DTO;
using LeaveCore.Models;
using LeaveCore.Models.Entities;
using LeaveCore.SharedFiles.Enum;
using Microsoft.EntityFrameworkCore;

namespace LeaveCore.BusinessLogic
{
    public class LeaveRequestService(LeaveContext db, IMapper mapper) : ILeaveRequestService
    {
        public async Task<List<LeaveRequestDTO>> GetByEmployeeAsync(int employeeId, int clientId, CancellationToken ct = default)
        {
            var list = await db.LeaveRequests
                .Where(r => r.ClientId == clientId && r.EmployeeId == employeeId && r.IsActive)
                .OrderByDescending(r => r.StartDate)
                .AsNoTracking()
                .ToListAsync(ct);
            return mapper.Map<List<LeaveRequestDTO>>(list);
        }

        public async Task<LeaveRequestDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default)
        {
            var entity = await db.LeaveRequests
                .Where(r => r.LeaveRequestId == id && r.ClientId == clientId && r.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            return entity == null ? null : mapper.Map<LeaveRequestDTO>(entity);
        }

        public async Task<LeaveRequestDTO?> SubmitAsync(LeaveRequestDTO dto, int clientId, string userId, CancellationToken ct = default)
        {
            if (dto.EmployeeId == null || dto.LeaveTypeId == null || dto.StartDate == null || dto.EndDate == null)
                return null;

            var days = dto.Days ?? CalculateDays(dto.StartDate.Value, dto.EndDate.Value);

            var entity = new LeaveRequest
            {
                ClientId = clientId,
                Code = await NextCodeAsync(clientId, ct),
                EmployeeId = dto.EmployeeId.Value,
                LeaveTypeId = dto.LeaveTypeId.Value,
                StartDate = dto.StartDate.Value,
                EndDate = dto.EndDate.Value,
                Days = days,
                Reason = dto.Reason,
                Status = LeaveStatusEnum.Pending,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };
            db.LeaveRequests.Add(entity);
            await db.SaveChangesAsync(ct);
            return mapper.Map<LeaveRequestDTO>(entity);
        }

        public async Task<bool> ApproveAsync(int id, int clientId, string approverId, CancellationToken ct = default)
        {
            using var tx = await db.Database.BeginTransactionAsync(ct);
            var request = await db.LeaveRequests
                .Where(r => r.LeaveRequestId == id && r.ClientId == clientId && r.IsActive)
                .FirstOrDefaultAsync(ct);
            if (request == null || request.Status != LeaveStatusEnum.Pending) return false;

            request.Status = LeaveStatusEnum.Approved;
            request.ApprovedBy = approverId;
            request.ApprovedDate = DateTime.UtcNow;

            // Bump the matching entitlement's UsedDays. If no entitlement exists
            // for this (employee, type, year) we still approve — the report
            // will show the deficit. Avoids rejecting requests over a missing
            // setup row.
            var year = request.StartDate.Year;
            var entitlement = await db.LeaveEntitlements
                .Where(e => e.ClientId == clientId
                    && e.EmployeeId == request.EmployeeId
                    && e.LeaveTypeId == request.LeaveTypeId
                    && e.Year == year
                    && e.IsActive)
                .FirstOrDefaultAsync(ct);
            if (entitlement != null)
                entitlement.UsedDays += request.Days;

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }

        public async Task<bool> RejectAsync(int id, string? reason, int clientId, string approverId, CancellationToken ct = default)
        {
            using var tx = await db.Database.BeginTransactionAsync(ct);
            var request = await db.LeaveRequests
                .Where(r => r.LeaveRequestId == id && r.ClientId == clientId && r.IsActive)
                .FirstOrDefaultAsync(ct);
            if (request == null) return false;

            // If the request had been Approved earlier and we're flipping to
            // Rejected, reverse the entitlement bump. Pending → Rejected does
            // not touch the entitlement.
            if (request.Status == LeaveStatusEnum.Approved)
                await ReverseEntitlementUsageAsync(request, clientId, ct);

            request.Status = LeaveStatusEnum.Rejected;
            request.ApprovedBy = approverId;
            request.ApprovedDate = DateTime.UtcNow;
            request.RejectionReason = reason;

            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }

        public async Task<bool> CancelAsync(int id, int clientId, CancellationToken ct = default)
        {
            using var tx = await db.Database.BeginTransactionAsync(ct);
            var request = await db.LeaveRequests
                .Where(r => r.LeaveRequestId == id && r.ClientId == clientId && r.IsActive)
                .FirstOrDefaultAsync(ct);
            if (request == null) return false;

            if (request.Status == LeaveStatusEnum.Approved)
                await ReverseEntitlementUsageAsync(request, clientId, ct);

            request.Status = LeaveStatusEnum.Cancelled;
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return true;
        }

        public async Task<SearchResponseDTO?> SearchAsync(LeaveRequestDTO model, int clientId, CancellationToken ct = default)
        {
            var query = db.LeaveRequests
                .Where(r => r.ClientId == clientId && r.IsActive
                    && (model.LeaveRequestId == null || r.LeaveRequestId == model.LeaveRequestId)
                    && (model.EmployeeId == null || r.EmployeeId == model.EmployeeId)
                    && (model.LeaveTypeId == null || r.LeaveTypeId == model.LeaveTypeId)
                    && (model.Status == null || r.Status == model.Status)
                    && (model.StartDate == null || r.StartDate >= model.StartDate)
                    && (model.EndDate == null || r.EndDate <= model.EndDate))
                .AsNoTracking();

            return new SearchResponseDTO
            {
                Total = await query.CountAsync(ct),
                Data = await query
                    .OrderByDescending(r => r.StartDate)
                    .Take(model.Take).Skip(model.Skip)
                    .ToListAsync(ct)
            };
        }

        private async Task ReverseEntitlementUsageAsync(LeaveRequest request, int clientId, CancellationToken ct)
        {
            var year = request.StartDate.Year;
            var entitlement = await db.LeaveEntitlements
                .Where(e => e.ClientId == clientId
                    && e.EmployeeId == request.EmployeeId
                    && e.LeaveTypeId == request.LeaveTypeId
                    && e.Year == year
                    && e.IsActive)
                .FirstOrDefaultAsync(ct);
            if (entitlement != null)
                entitlement.UsedDays -= request.Days;
        }

        private async Task<int> NextCodeAsync(int clientId, CancellationToken ct)
        {
            var max = await db.LeaveRequests
                .Where(r => r.ClientId == clientId)
                .OrderByDescending(r => r.Code)
                .Select(r => (int?)r.Code)
                .FirstOrDefaultAsync(ct);
            return (max ?? 0) + 1;
        }

        /// <summary>
        /// Inclusive day count between two dates. Sprint 2.7 ships the simplest
        /// possible formula — every calendar day counts. A future iteration
        /// can subtract weekends and per-tenant holidays once HolidayCore (or
        /// a TuningCore extension) lands.
        /// </summary>
        private static decimal CalculateDays(DateTime start, DateTime end)
        {
            if (end < start) return 0;
            return (end.Date - start.Date).Days + 1;
        }
    }
}
