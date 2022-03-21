using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Graphql;

public class Query
{
    public IEnumerable<Participant> GetParticipants([Service] ApplicationDbContext db) => db.Participants;
}

public class Mutation
{
    public Participant AddParticipant([Service] ApplicationDbContext dbContext,
        [Service] IHttpContextAccessor httpContextAccessor, NewParticipant newParticipant)
    {
        var participant = MapperUtil.Mapper.Map<Participant>(newParticipant);
        participant.SignUpDate = DateTime.Now;
        participant.VariableSymbol = dbContext.Participants.Any()
            ? (int.Parse(dbContext.Participants.Last().VariableSymbol) + 1).ToString()
            : $"{DateTime.Now.Year}001";
        participant.Ip = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString();
        
        dbContext.Participants.Add(participant);
        dbContext.SaveChanges();
        return participant;
    }
}