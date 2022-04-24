using prijimacky_backend.Data;
using prijimacky_backend.Graphql.Types;

namespace prijimacky_backend.Services;

public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _db;
    public StatisticsService(ApplicationDbContext db) => _db = db;

    public Statistics Get()
    {
        var signupCount = _db.Participants.Count();
        var remaining = _db.Settings.Capacity - signupCount;
        var remainingOver = remaining switch
        {
            >= 0 => _db.Settings.AllowedOver,
            // Adds negative to positive, thus reducing the number
            _ => _db.Settings.AllowedOver + remaining
        };

        return new Statistics(
            signupCount,
            _db.Settings.Capacity,
            remaining >= 0 ? remaining : 0,
            remainingOver,
            0
        );
    }
}