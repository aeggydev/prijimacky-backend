using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Services;

public class ParticipantService : IParticipantService
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _email;

    public ParticipantService(ApplicationDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public IEnumerable<Participant> GetAll() => _db.Participants;

    public Participant? GetById(int id) => _db.Participants.Find(id);

    public Participant Create(NewParticipant newParticipant, string ip)
    {
        var participant = MapperUtil.Mapper.Map<Participant>(newParticipant);
        participant.SignUpDate = DateTime.Now;
        participant.VariableSymbol = _db.Participants.Any()
            ? (int.Parse(_db.Participants.OrderBy(p => p.Id).Last().VariableSymbol) + 1).ToString()
            : $"{DateTime.Now.Year}001";
        participant.Ip = ip;

        _db.Participants.Add(participant);
        _db.SaveChanges();
        return participant;
    }

    public Participant Update(int id, UpdateParticipant input)
    {
        var toMerge = _db.Participants.Find(id);
        if (toMerge is null) throw new Exception("Id not found");

        var merged = MapperUtil.Mapper.Map(input, toMerge)!;

        var entry = _db.Entry(toMerge);
        entry.CurrentValues.SetValues(merged);
        _db.SaveChanges();

        return entry.Entity;
    }

    public IEnumerable<Participant> UpdateMultiple(IEnumerable<UpdateParticipantsItem> inputs)
    {
        List<Participant> entryAccumulator = new();
        foreach (var (id, updateParticipant) in inputs)
        {
            var entry = Update(id, updateParticipant);
            entryAccumulator.Add(entry);
        }

        _db.SaveChanges();
        return entryAccumulator;
    }

    public bool Delete(int id)
    {
        var participant = _db.Participants.Find(id);
        if (participant is null) return false;
        _db.Participants.Remove(participant);
        _db.SaveChanges();
        return true;
    }

    public bool ConfirmPayment(int id)
    {
        var participant = _db.Participants.Find(id);
        if (participant is null) return false;
        if (participant.Status != ParticipantStatus.PaidUnconfirmed) return false;
        // TODO: Send email
        participant.PaidNotified = true;
        _db.SaveChanges();
        return true;
    }

    public bool ConfirmLateCancel(int id)
    {
        var participant = _db.Participants.Find(id);
        if (participant is null) return false;
        if (participant.Status != ParticipantStatus.UnpaidLate) return false;
        // TODO: Send email
        participant.CancelationNotified = true;
        _db.SaveChanges();
        return true;
    }

    public bool ForceCancel(int id, bool status)
    {
        var participant = _db.Participants.Find(id);
        if (participant is null) return false;
        // TODO: Send email
        participant.CancelationNotified = status;
        _db.SaveChanges();
        return true;
    }
}