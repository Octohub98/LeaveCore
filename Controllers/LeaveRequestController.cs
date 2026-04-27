using LeaveCore.BusinessLogic;
using LeaveCore.DTO;
using Microsoft.AspNetCore.Mvc;
using Octohub.Core.Controllers;

namespace LeaveCore.Controllers
{
    public class LeaveRequestController(ILeaveRequestService service) : OctohubControllerBase
    {
        [HttpGet("ByEmployee/{employeeId:int}")]
        public async Task<IActionResult> GetByEmployee(int employeeId, CancellationToken ct)
            => Ok(await service.GetByEmployeeAsync(employeeId, ClientId!.Value, ct));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var dto = await service.GetByIdAsync(id, ClientId!.Value, ct);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Submit([FromBody] LeaveRequestDTO dto, CancellationToken ct)
        {
            var saved = await service.SubmitAsync(dto, ClientId!.Value, UserId, ct);
            return saved == null ? BadRequest() : Ok(saved);
        }

        [HttpPost("{id:int}/Approve")]
        public async Task<IActionResult> Approve(int id, CancellationToken ct)
        {
            var ok = await service.ApproveAsync(id, ClientId!.Value, UserId, ct);
            return ok ? Ok() : BadRequest(new { message = "Approval failed (not found or not in Pending status)." });
        }

        [HttpPost("{id:int}/Reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] ApproveRejectRequest body, CancellationToken ct)
        {
            var ok = await service.RejectAsync(id, body?.Reason, ClientId!.Value, UserId, ct);
            return ok ? Ok() : NotFound();
        }

        [HttpPost("{id:int}/Cancel")]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var ok = await service.CancelAsync(id, ClientId!.Value, ct);
            return ok ? Ok() : NotFound();
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search([FromBody] LeaveRequestDTO model, CancellationToken ct)
        {
            var result = await service.SearchAsync(model, ClientId!.Value, ct);
            return result == null ? BadRequest() : Ok(result);
        }
    }
}
