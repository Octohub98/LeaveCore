using AutoMapper;
using LeaveCore.DTO;
using LeaveCore.Models;
using LeaveCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveCore.BusinessLogic
{
    public class LeaveTypeService(LeaveContext db, IMapper mapper) : ILeaveTypeService
    {
        public async Task<List<LeaveTypeDTO>> GetAllAsync(int clientId, CancellationToken ct = default)
        {
            var list = await db.LeaveTypes
                .Where(t => t.ClientId == clientId && t.IsActive)
                .OrderBy(t => t.Name)
                .AsNoTracking()
                .ToListAsync(ct);
            return mapper.Map<List<LeaveTypeDTO>>(list);
        }

        public async Task<LeaveTypeDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default)
        {
            var entity = await db.LeaveTypes
                .Where(t => t.LeaveTypeId == id && t.ClientId == clientId && t.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            return entity == null ? null : mapper.Map<LeaveTypeDTO>(entity);
        }

        public async Task<LeaveTypeDTO?> InsertAsync(LeaveTypeDTO dto, int clientId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(dto.Name)) return null;
            var entity = new LeaveType
            {
                ClientId = clientId,
                Code = await NextCodeAsync(clientId, ct),
                Name = dto.Name,
                DefaultEntitlementDays = dto.DefaultEntitlementDays ?? 0,
                IsPaid = dto.IsPaid ?? true,
                AllowsCarryOver = dto.AllowsCarryOver ?? false,
                MaxCarryOverDays = dto.MaxCarryOverDays,
                Notes = dto.Notes,
                IsActive = true,
            };
            db.LeaveTypes.Add(entity);
            await db.SaveChangesAsync(ct);
            return mapper.Map<LeaveTypeDTO>(entity);
        }

        public async Task<LeaveTypeDTO?> UpdateAsync(LeaveTypeDTO dto, int clientId, CancellationToken ct = default)
        {
            if (dto.LeaveTypeId == null) return null;
            var entity = await db.LeaveTypes
                .Where(t => t.LeaveTypeId == dto.LeaveTypeId && t.ClientId == clientId)
                .FirstOrDefaultAsync(ct);
            if (entity == null) return null;

            if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
            if (dto.DefaultEntitlementDays != null) entity.DefaultEntitlementDays = dto.DefaultEntitlementDays.Value;
            if (dto.IsPaid != null) entity.IsPaid = dto.IsPaid.Value;
            if (dto.AllowsCarryOver != null) entity.AllowsCarryOver = dto.AllowsCarryOver.Value;
            entity.MaxCarryOverDays = dto.MaxCarryOverDays;
            entity.Notes = dto.Notes;

            await db.SaveChangesAsync(ct);
            return mapper.Map<LeaveTypeDTO>(entity);
        }

        public async Task<bool> DeleteAsync(int id, int clientId, CancellationToken ct = default)
        {
            var entity = await db.LeaveTypes
                .Where(t => t.LeaveTypeId == id && t.ClientId == clientId && t.IsActive)
                .FirstOrDefaultAsync(ct);
            if (entity == null) return false;
            entity.IsActive = false;
            await db.SaveChangesAsync(ct);
            return true;
        }

        private async Task<int> NextCodeAsync(int clientId, CancellationToken ct)
        {
            var max = await db.LeaveTypes
                .Where(t => t.ClientId == clientId)
                .OrderByDescending(t => t.Code)
                .Select(t => (int?)t.Code)
                .FirstOrDefaultAsync(ct);
            return (max ?? 0) + 1;
        }
    }
}
