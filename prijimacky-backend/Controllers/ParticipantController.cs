using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using prijimacky_backend.Data;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Controllers;

public record NewParticipant(
    string ParticipantName,
    string ParticipantSurname,
    string ParentName,
    string ParentSurname,
    string School,
    string Phone,
    string Email);

[ApiController]
[Route("[controller]")]
public class ParticipantController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mapper _mapper;

    public ParticipantController(ApplicationDbContext dbContext)
    {
        // TODO: Move automapper config elsewhere
        // TODO: Configure the whole object from here
        var config = new MapperConfiguration(cfg =>
            cfg.CreateMap<NewParticipant, Participant>()
        );
        _mapper = new Mapper(config);

        _dbContext = dbContext;
    }

    [HttpPost]
    public Participant SignUp(NewParticipant newParticipant)
    {
        var participant = MapperUtil.Mapper.Map<Participant>(newParticipant);
        participant.SignUpDate = DateTime.Now;
        participant.VariableSymbol = _dbContext.Participants.Any()
            ? (int.Parse(_dbContext.Participants.Last().VariableSymbol) + 1).ToString()
            : $"{DateTime.Now.Year}001";
        participant.Ip = HttpContext.Connection.RemoteIpAddress!.ToString();
        _dbContext.Participants.Add(participant);
        _dbContext.SaveChanges();
        Console.WriteLine(JsonSerializer.Serialize(newParticipant));
        return participant;
    }
}