using Microsoft.EntityFrameworkCore;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Participant> Participants { get; set; } = default!;

    private DbSet<Settings> SettingsMultirow { get; set; } = default!;
    public Settings Settings
    {
        get
        {
            var settings = SettingsMultirow.SingleOrDefault();
            if (settings is not null) return settings;

            SettingsMultirow.Add(new Settings());
            SaveChanges();
            return SettingsMultirow.Single();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseInMemoryDatabase("Application");
    }
}