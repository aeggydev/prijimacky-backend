using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Graphql;

public class Query
{
    public IEnumerable<Participant> GetParticipants([Service] ApplicationDbContext db) => db.Participants;
    public Settings GetSettings([Service] ApplicationDbContext db) => db.Settings;
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
    public Participant UpdateParticipant([Service] ApplicationDbContext dbContext, int id, UpdateParticipant updateParticipant)
    {
        var toMerge = dbContext.Participants.Find(id);
        if (toMerge is null) throw new Exception("Id not found");
        
        var merged = MapperUtil.Mapper.Map(updateParticipant, toMerge)!;
        
        var entry = dbContext.Entry(toMerge);
        entry.CurrentValues.SetValues(merged);
        dbContext.SaveChanges();
        
        return entry.Entity;
    }

    public Settings UpdateSettings([Service] ApplicationDbContext dbContext, UpdateSettings updateSettings)
    {
        var toMerge = dbContext.Settings;
        var merged = MapperUtil.Mapper.Map(updateSettings, toMerge)!;
        var entry = dbContext.Entry(toMerge);
        entry.CurrentValues.SetValues(merged);
        dbContext.SaveChanges();

        return entry.Entity;
    }
}