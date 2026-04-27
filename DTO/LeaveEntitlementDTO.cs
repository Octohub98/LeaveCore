namespace LeaveCore.DTO
{
    public class LeaveEntitlementDTO : SearchDTO
    {
        public int? LeaveEntitlementId { get; set; }
        public int? ClientId { get; set; }
        public int? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? Year { get; set; }
        public decimal? AllocatedDays { get; set; }
        public decimal? UsedDays { get; set; }
        public decimal? CarryOverFromPreviousYear { get; set; }
        public bool? IsActive { get; set; } = true;

        // Computed at read time, never persisted.
        public decimal RemainingDays => (AllocatedDays ?? 0) + (CarryOverFromPreviousYear ?? 0) - (UsedDays ?? 0);
    }
}
