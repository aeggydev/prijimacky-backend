using Microsoft.EntityFrameworkCore;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Participant> Participants { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Date Source=Application.db");
    }
}