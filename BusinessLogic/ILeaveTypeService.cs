using LeaveCore.DTO;

namespace LeaveCore.BusinessLogic
{
    public interface ILeaveTypeService
    {
        Task<List<LeaveTypeDTO>> GetAllAsync(int clientId, CancellationToken ct = default);
        Task<LeaveTypeDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default);
        Task<LeaveTypeDTO?> InsertAsync(LeaveTypeDTO dto, int clientId, CancellationToken ct = default);
        Task<LeaveTypeDTO?> UpdateAsync(LeaveTypeDTO dto, int clientId, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, int clientId, CancellationToken ct = default);
    }
}
