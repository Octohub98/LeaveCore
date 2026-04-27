using AutoMapper;
using LeaveCore.DTO;
using LeaveCore.Models;
using LeaveCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveCore.BusinessLogic
{
    public class LeaveEntitlementService(LeaveContext db, IMapper mapper) : ILeaveEntitlementService
    {
        public async Task<List<LeaveEntitlementDTO>> GetByEmployeeAsync(int employeeId, int? year, int clientId, CancellationToken ct = default)
        {
            var query = db.LeaveEntitlements
                .Where(e => e.ClientId == clientId && e.EmployeeId == employeeId && e.IsActive);

            if (year != null) query = query.Where(e => e.Year == year);

            var list = await query
                .OrderByDescending(e => e.Year)
                .ThenBy(e => e.LeaveTypeId)
                .AsNoTracking()
                .ToListAsync(ct);
            return mapper.Map<List<LeaveEntitlementDTO>>(list);
        }

        public async Task<LeaveEntitlementDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default)
        {
            var entity = await db.LeaveEntitlements
                .Where(e => e.LeaveEntitlementId == id && e.ClientId == clientId && e.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            return entity == null ? null : mapper.Map<LeaveEntitlementDTO>(entity);
        }

        public async Task<LeaveEntitlementDTO?> InsertAsync(LeaveEntitlementDTO dto, int clientId, CancellationToken ct = default)
        {
            if (dto.EmployeeId == null || dto.LeaveTypeId == null || dto.Year == null || dto.AllocatedDays == null)
                return null;

            var entity = new LeaveEntitlement
            {
                ClientId = clientId,
                Code = await NextCodeAsync(clientId, ct),
                EmployeeId = dto.EmployeeId.Value,
                LeaveTypeId = dto.LeaveTypeId.Value,
                Year = dto.Year.Value,
                AllocatedDays = dto.AllocatedDays.Value,
                UsedDays = 0,
                CarryOverFromPreviousYear = dto.CarryOverFromPreviousYear ?? 0,
                IsActive = true,
            };
            db.LeaveEntitlements.Add(entity);
            await db.SaveChangesAsync(ct);
            return mapper.Map<LeaveEntitlementDTO>(entity);
        }

        public async Task<LeaveEntitlementDTO?> UpdateAsync(LeaveEntitlementDTO dto, int clientId, CancellationToken ct = default)
        {
            if (dto.LeaveEntitlementId == null) return null;
            var entity = await db.LeaveEntitlements
                .Where(e => e.LeaveEntitlementId == dto.LeaveEntitlementId && e.ClientId == clientId)
                .FirstOrDefaultAsync(ct);
            if (entity == null) return null;

            if (dto.AllocatedDays != null) entity.AllocatedDays = dto.AllocatedDays.Value;
            if (dto.CarryOverFromPreviousYear != null) entity.CarryOverFromPreviousYear = dto.CarryOverFromPreviousYear.Value;

            // UsedDays is managed by the LeaveRequest workflow — we deliberately
            // don't accept it from the client to prevent drift.
            await db.SaveChangesAsync(ct);
            return mapper.Map<LeaveEntitlementDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id, int clientId, CancellationToken ct = default)
        {
            var entity = await db.LeaveEntitlements
                .Where(e => e.LeaveEntitlementId == id && e.ClientId == clientId && e.IsActive)
                .FirstOrDefaultAsync(ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await db.SaveChangesAsync(ct);
            return true;
        }

        private async Task<int> NextCodeAsync(int clientId, CancellationToken ct)
        {
            var max = await db.LeaveEntitlements
                .Where(e => e.ClientId == clientId)
                .OrderByDescending(e => e.Code)
                .Select(e => (int?)e.Code)
                .FirstOrDefaultAsync(ct);
            return (max ?? 0) + 1;
        }
    }
}
