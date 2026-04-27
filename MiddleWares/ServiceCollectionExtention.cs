using LeaveCore.BusinessLogic;

namespace LeaveCore.MiddleWares
{
    public static class ServiceCollectionExtention
    {
        public static IServiceCollection AddDIService(this IServiceCollection services)
        {
            services.AddScoped<ILeaveTypeService, LeaveTypeService>();
            services.AddScoped<ILeaveEntitlementService, LeaveEntitlementService>();
            services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            return services;
        }
    }
}
