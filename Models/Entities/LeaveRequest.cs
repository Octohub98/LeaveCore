using LeaveCore.SharedFiles.Enum;
using Octohub.Core.Abstractions.Entities;

namespace LeaveCore.Models.Entities
{
    /// <summary>
    /// A leave request submitted by an employee. Goes through Pending →
    /// Approved/Rejected/Cancelled. On Approve, LeaveEntitlement.UsedDays
    /// is bumped; on Reject/Cancel from Approved, it is reversed.
    /// </summary>
    public class LeaveRequest : IHasClientId, IHasCode, IActivatable
    {
        public int LeaveRequestId { get; set; }
        public int ClientId { get; set; }
        public int Code { get; set; }
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Days { get; set; }
        public string? Reason { get; set; }
        public LeaveStatusEnum Status { get; set; } = LeaveStatusEnum.Pending;
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public LeaveType LeaveType { get; set; } = default!;
    }
}
