namespace LeaveCore.DTO
{
    public class LeaveTypeDTO : SearchDTO
    {
        public int? LeaveTypeId { get; set; }
        public int? ClientId { get; set; }
        public int? Code { get; set; }
        public string? Name { get; set; }
        public decimal? DefaultEntitlementDays { get; set; }
        public bool? IsPaid { get; set; }
        public bool? AllowsCarryOver { get; set; }
        public decimal? MaxCarryOverDays { get; set; }
        public string? Notes { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
