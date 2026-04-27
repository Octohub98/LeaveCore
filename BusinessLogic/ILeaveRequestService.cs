using LeaveCore.DTO;

namespace LeaveCore.BusinessLogic
{
    public interface ILeaveRequestService
    {
        Task<List<LeaveRequestDTO>> GetByEmployeeAsync(int employeeId, int clientId, CancellationToken ct = default);
        Task<LeaveRequestDTO?> GetByIdAsync(int id, int clientId, CancellationToken ct = default);
        Task<LeaveRequestDTO?> SubmitAsync(LeaveRequestDTO dto, int clientId, string userId, CancellationToken ct = default);
        Task<bool> ApproveAsync(int id, int clientId, string approverId, CancellationToken ct = default);
        Task<bool> RejectAsync(int id, string? reason, int clientId, string approverId, CancellationToken ct = default);
        Task<bool> CancelAsync(int id, int clientId, CancellationToken ct = default);
        Task<SearchResponseDTO?> SearchAsync(LeaveRequestDTO model, int clientId, CancellationToken ct = default);
    }
}
