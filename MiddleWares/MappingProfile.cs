using AutoMapper;
using LeaveCore.DTO;
using LeaveCore.Models.Entities;

namespace LeaveCore.MiddleWares
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<LeaveType, LeaveTypeDTO>().ReverseMap();
            CreateMap<LeaveEntitlement, LeaveEntitlementDTO>().ReverseMap();
            CreateMap<LeaveRequest, LeaveRequestDTO>().ReverseMap();
        }
    }
}
