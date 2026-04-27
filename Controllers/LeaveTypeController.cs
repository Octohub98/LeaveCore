using LeaveCore.BusinessLogic;
using LeaveCore.DTO;
using Microsoft.AspNetCore.Mvc;
using Octohub.Core.Controllers;

namespace LeaveCore.Controllers
{
    public class LeaveTypeController(ILeaveTypeService service) : OctohubControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await service.GetAllAsync(ClientId!.Value, ct));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var dto = await service.GetByIdAsync(id, ClientId!.Value, ct);
            return dto == null ? NotFound() : Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] LeaveTypeDTO dto, CancellationToken ct)
        {
            var saved = await service.InsertAsync(dto, ClientId!.Value, ct);
            return saved == null ? BadRequest() : Ok(saved);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LeaveTypeDTO dto, CancellationToken ct)
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
