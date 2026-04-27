using LeaveCore.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeaveCore.Models
{
    public class LeaveContext : DbContext
    {
        public LeaveContext(DbContextOptions<LeaveContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveType>(e =>
            {
                e.Property(t => t.DefaultEntitlementDays).HasColumnType("numeric(7,2)");
                e.Property(t => t.MaxCarryOverDays).HasColumnType("numeric(7,2)");
                e.HasIndex(t => new { t.ClientId, t.Code }).IsUnique();
            });

            modelBuilder.Entity<LeaveEntitlement>(e =>
            {
                e.Property(t => t.AllocatedDays).HasColumnType("numeric(7,2)");
                e.Property(t => t.UsedDays).HasColumnType("numeric(7,2)");
                e.Property(t => t.CarryOverFromPreviousYear).HasColumnType("numeric(7,2)");
                e.HasIndex(t => new { t.ClientId, t.EmployeeId, t.LeaveTypeId, t.Year }).IsUnique();
                e.HasOne(en => en.LeaveType)
                    .WithMany(lt => lt.Entitlements)
                    .HasForeignKey(en => en.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LeaveRequest>(e =>
            {
                e.Property(r => r.Days).HasColumnType("numeric(7,2)");
                e.HasIndex(r => new { r.ClientId, r.EmployeeId, r.Status });
                e.HasIndex(r => new { r.ClientId, r.StartDate, r.EndDate });
                e.HasOne(r => r.LeaveType)
                    .WithMany(lt => lt.Requests)
                    .HasForeignKey(r => r.LeaveTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<LeaveType> LeaveTypes { get; set; } = default!;
        public DbSet<LeaveEntitlement> LeaveEntitlements { get; set; } = default!;
        public DbSet<LeaveRequest> LeaveRequests { get; set; } = default!;
    }
}
