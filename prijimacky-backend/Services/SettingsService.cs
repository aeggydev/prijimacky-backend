using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Services;

public class SettingsService : ISettingsService
{
    private readonly ApplicationDbContext _db;
    public SettingsService(ApplicationDbContext db) => _db = db;

    public Settings Get() => _db.Settings;

    public Settings Update(UpdateSettings input)
    {
        // TODO: Add validation
        var entry = _db.Entry(_db.Settings);
        entry.CurrentValues.SetValues(input);
        _db.SaveChanges();

        return entry.Entity;
    }
}