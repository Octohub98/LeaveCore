using Octohub.Core.Abstractions.Entities;

namespace LeaveCore.Models.Entities
{
    /// <summary>
    /// Annual allocation per (employee, leave type, year). UsedDays is
    /// denormalized — the LeaveRequestService bumps it on Approve and
    /// reverses on Reject/Cancel. Remaining is computed at read time.
    /// </summary>
    public class LeaveEntitlement : IHasClientId, IHasCode, IActivatable
    {
        public int LeaveEntitlementId { get; set; }
        public int ClientId { get; set; }
        public int Code { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public decimal AllocatedDays { get; set; }
        public decimal UsedDays { get; set; }
        public decimal CarryOverFromPreviousYear { get; set; }
        public bool IsActive { get; set; } = true;
        public LeaveType LeaveType { get; set; } = default!;
    }
}
