using LeaveCore.BusinessLogic;
using LeaveCore.DTO;
using Microsoft.AspNetCore.Mvc;
using Octohub.Core.Controllers;

namespace LeaveCore.Controllers
{
    public class LeaveEntitlementController(ILeaveEntitlementService service) : OctohubControllerBase
    {
        [HttpGet("ByEmployee/{employeeId:int}")]
        public async Task<IActionResult> GetByEmployee(int employeeId, [FromQuery] int? year, CancellationToken ct)
            => Ok(await service.GetByEmployeeAsync(employeeId, year, ClientId!.Value, ct));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var dto = await service.GetByIdAsync(id, ClientId!.Value, ct);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] LeaveEntitlementDTO dto, CancellationToken ct)
        {
            var saved = await service.InsertAsync(dto, ClientId!.Value, ct);
            return saved == null ? BadRequest() : Ok(saved);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LeaveEntitlementDTO dto, CancellationToken ct)
        {
            var saved = await service.UpdateAsync(dto, ClientId!.Value, ct);
            return saved == null ? NotFound() : Ok(saved);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await service.DeleteAsync(id, ClientId!.Value, ct);
            return ok ? Ok() : NotFound();
        }
    }
}
