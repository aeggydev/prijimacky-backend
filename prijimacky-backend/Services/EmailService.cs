using prijimacky_backend.Data;
using prijimacky_backend.Entities;
using prijimacky_backend.Graphql.Types;

namespace prijimacky_backend.Services;

public class EmailService : IEmailService
{
    private readonly ApplicationDbContext _db;
    public EmailService(ApplicationDbContext db) => _db = db;

    public EmailStatistics GetStatistics()
    {
        var participants = _db.Participants.ToList();
        return new EmailStatistics(
            participants.Count(x => x.Status == ParticipantStatus.PaidConfirmed),
            participants.Count(x => x.Status == ParticipantStatus.PaidUnconfirmed),
            participants.Count(x => x.Status == ParticipantStatus.UnpaidLate),
            participants.Count(x => x.Status == ParticipantStatus.Unpaid),
            participants.Count(x => x.Status == ParticipantStatus.Canceled),
            0); // TODO: Add actual free spot counting
    }
}