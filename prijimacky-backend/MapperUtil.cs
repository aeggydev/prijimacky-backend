using AutoMapper;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend;

public static class MapperUtil
{
    public static Mapper Mapper { get; }

    static MapperUtil()
    {
        var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NewParticipant, Participant>();
                cfg.CreateMap<UpdateParticipant, Participant>()
                    .ForMember(dest => dest.DueDate, opt => opt.MapFrom((src, dest) => src.DueDate ?? dest.DueDate))
                    .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
                cfg.CreateMap<UpdateSettings, Settings>()
                    .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null));
            }
        );
        Mapper = new Mapper(config);
    }
}