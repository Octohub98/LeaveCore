using LeaveCore.SharedFiles.Enum;

namespace LeaveCore.DTO
{
    public class LeaveRequestDTO : SearchDTO
    {
        public int? LeaveRequestId { get; set; }
        public int? ClientId { get; set; }
        public int? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Days { get; set; }
        public string? Reason { get; set; }
        public LeaveStatusEnum? Status { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class ApproveRejectRequest
    {
        public string? Reason { get; set; } // optional rejection reason
    }
}
