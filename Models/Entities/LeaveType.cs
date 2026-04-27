using Octohub.Core.Abstractions.Entities;

namespace LeaveCore.Models.Entities
{
    /// <summary>
    /// Per-tenant catalog of leave types (Annual, Sick, Maternity, Unpaid,
    /// Custom...). Each tenant defines their own list and the default
    /// entitlement that applies when an employee is hired.
    /// </summary>
    public class LeaveType : IHasClientId, IHasCode, IActivatable
    {
        public int LeaveTypeId { get; set; }
        public int ClientId { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public decimal DefaultEntitlementDays { get; set; }
        public bool IsPaid { get; set; } = true;
        public bool AllowsCarryOver { get; set; }
        public decimal? MaxCarryOverDays { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<LeaveEntitlement> Entitlements { get; set; } = [];
        public ICollection<LeaveRequest> Requests { get; set; } = [];
    }
}
