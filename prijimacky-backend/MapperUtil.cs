using AutoMapper;
using prijimacky_backend.Controllers;
using prijimacky_backend.Entities;

namespace prijimacky_backend;

public static class MapperUtil
{
    public static Mapper Mapper { get; }

    static MapperUtil()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.CreateMap<NewParticipant, Participant>()
        );
        Mapper = new Mapper(config);
    }
}