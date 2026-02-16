using AutoMapper;
using childspace_backend.Models;
using childspace_backend.Models.DTOs;
using childspace_backend.Models.Enums;

namespace childspace_backend.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<Child, ChildDto>()
                .ForMember(dest => dest.Groups, opt => opt.MapFrom(src => src.GroupChildren))
                .ForMember(dest => dest.Attendances, opt => opt.MapFrom(src => src.Attendances));

            CreateMap<GroupChild, GroupChildDto>();

            CreateMap<Center, CenterDto>();
 
            CreateMap<Schedule, ScheduleDto>();

            CreateMap<Attendance, AttendanceDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Material, MaterialDto>();

            CreateMap<Message, MessageDto>();

            CreateMap<Chat, ChatDto>();

            CreateMap<TrialRequest, TrialRequestDto>();

            CreateMap<Subject, SubjectDto>();

            CreateMap<UserChat, UserChatDto>();

            CreateMap<Group, GroupDto>();
        }
    }
}
