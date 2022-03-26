using Microsoft.EntityFrameworkCore.ChangeTracking;
using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;
using prijimacky_backend.Graphql.Types;

namespace prijimacky_backend.Graphql;

public class Query
{
    public IEnumerable<Participant> GetParticipants([Service] ApplicationDbContext db) => db.Participants;
    public Settings GetSettings([Service] ApplicationDbContext db) => db.Settings;

    public Statistics GetStatistics([Service] ApplicationDbContext db)
    {
        var signupCount = db.Participants.Count();
        var remaining = db.Settings.Capacity - signupCount;
        var remainingOver = remaining switch
        {
            >= 0 => db.Settings.AllowedOver,
            // Adds negative to positive, thus reducing the number
            _ => db.Settings.AllowedOver + remaining
        };

        return new Statistics(
            signupCount,
            remaining >= 0 ? remaining : 0,
            remainingOver,
            0
        );
    }
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

    public Participant UpdateParticipant([Service] ApplicationDbContext dbContext, int id,
        UpdateParticipant updateParticipant)
    {
        var toMerge = dbContext.Participants.Find(id);
        if (toMerge is null) throw new Exception("Id not found");

        var merged = MapperUtil.Mapper.Map(updateParticipant, toMerge)!;

        var entry = dbContext.Entry(toMerge);
        entry.CurrentValues.SetValues(merged);
        dbContext.SaveChanges();

        return entry.Entity;
    }

    public IEnumerable<Participant> UpdateParticipants([Service] ApplicationDbContext dbContext,
        List<UpdateParticipantsItem> updateParticipants)
    {
        List<EntityEntry<Participant>> entryAccumulator = new();
        foreach (var (id, updateParticipant) in updateParticipants)
        {
            var toMerge = dbContext.Participants.Find(id);
            if (toMerge is null) throw new Exception("Id not found");

            var merged = MapperUtil.Mapper.Map(updateParticipant, toMerge)!;
            
            var entry = dbContext.Entry(toMerge);
            entry.CurrentValues.SetValues(merged);
            entryAccumulator.Add(entry);
        }

        dbContext.SaveChanges();
        return entryAccumulator.Select(x => x.Entity);
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