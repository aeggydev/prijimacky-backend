﻿using Microsoft.EntityFrameworkCore;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Data;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Admin> Admins { get; set; } = default!;
    public DbSet<Participant> Participants { get; set; } = default!;

    private DbSet<Settings> SettingsMultirow { get; set; } = default!;
    public Settings Settings => SettingsMultirow.SingleOrDefault() ?? throw new Exception("Settings is null");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Create single settings row
        modelBuilder.Entity<Settings>().HasData(new Settings { Id = -1 });
        // TODO: Remove this for a configuration tool
        modelBuilder.Entity<Admin>().HasData(new Admin
            { Id = -1, Username = "default_admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("default_password") });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Filename=TestDatabase.db");
    }
}