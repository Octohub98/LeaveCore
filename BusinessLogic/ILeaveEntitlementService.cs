using LeaveCore.DTO;

namespace LeaveCore.BusinessLogic
{
    public interface ILeaveEntitlementService
    {
        Task<List<LeaveEntitlementDTO>> GetByEmployeeAsync(int employeeId, int? year, int clientId, CancellationToken ct = default);
        Task<LeaveEntitlementDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default);
        Task<LeaveEntitlementDTO?> InsertAsync(LeaveEntitlementDTO dto, int clientId, CancellationToken ct = default);
        Task<LeaveEntitlementDTO?> UpdateAsync(LeaveEntitlementDTO dto, int clientId, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, int clientId, CancellationToken ct = default);
    }
}
