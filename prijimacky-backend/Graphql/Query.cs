using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;
using prijimacky_backend.Graphql.Types;

namespace prijimacky_backend.Graphql;

public class Query
{
    public string Login([Service] ApplicationDbContext db, [Service] IConfiguration config, LoginInfo login)
    {
        var admin = db.Admins
            .FirstOrDefault(a => a.Username == login.Username);
        if (admin is null) throw new Exception("Invalid username");
        
        if (!admin.CheckPassword(login.Password)) throw new Exception("Invalid password");

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Secret"]));
        var authClaims = new List<Claim>
        {
            new("sub", login.Username),
            new(ClaimTypes.Role, "Admin")
        };
        var token = new JwtSecurityToken(
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            expires: DateTime.Now.AddDays(14),
            claims: authClaims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize(Roles = new[] { "Admin" })]
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
            db.Settings.Capacity,
            remaining >= 0 ? remaining : 0,
            remainingOver,
            0
        );
    }

    public EmailStatistics GetEmailStatistics([Service] ApplicationDbContext db)
    {
        var participants = db.Participants.ToList();
        return new EmailStatistics(
            participants.Count(x => x.Status == ParticipantStatus.PaidConfirmed),
            participants.Count(x => x.Status == ParticipantStatus.PaidUnconfirmed),
            participants.Count(x => x.Status == ParticipantStatus.UnpaidLate),
            participants.Count(x => x.Status == ParticipantStatus.Unpaid),
            participants.Count(x => x.Status == ParticipantStatus.Canceled),
            0); // TODO: Add actual free spot counting
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
            ? (int.Parse(dbContext.Participants.OrderBy(p => p.Id).Last().VariableSymbol) + 1).ToString()
            : $"{DateTime.Now.Year}001";
        participant.Ip = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString();

        dbContext.Participants.Add(participant);
        dbContext.SaveChanges();
        return participant;
    }

    [Authorize(Roles = new[] { "Admin" })]
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

    [Authorize(Roles = new[] { "Admin" })]
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

    [Authorize(Roles = new[] { "Admin" })]
    public Settings UpdateSettings([Service] ApplicationDbContext dbContext, UpdateSettings updateSettings)
    {
        var entry = dbContext.Entry(dbContext.Settings);
        entry.CurrentValues.SetValues(updateSettings);
        dbContext.SaveChanges();

        return entry.Entity;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public int AddAdmin([Service] ApplicationDbContext db, LoginInfo login)
    {
        if (db.Admins.Any(x => x.Username == login.Username))
            throw new Exception("Username already exists");

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hash = BCrypt.Net.BCrypt.HashPassword(login.Password, salt);
        var admin = new Admin
        {
            PasswordHash = hash,
            Username = login.Username
        };
        db.Admins.Add(admin);
        db.SaveChanges();
        return admin.Id;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public bool RemoveParticipant([Service] ApplicationDbContext db, int id)
    {
        var participant = db.Participants.Find(id);
        if (participant is null) return false;
        db.Participants.Remove(participant);
        db.SaveChanges();
        return true;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public bool ConfirmPayment([Service] ApplicationDbContext db, int id)
    {
        var participant = db.Participants.Find(id);
        if (participant is null) return false;
        if (participant.Status != ParticipantStatus.PaidUnconfirmed) return false;
        // TODO: Send email
        participant.PaidNotified = true;
        db.SaveChanges();
        return true;
    }
    
    
    [Authorize(Roles = new[] { "Admin" })]
    public bool ConfirmLateCancel([Service] ApplicationDbContext db, int id)
    {
        var participant = db.Participants.Find(id);
        if (participant is null) return false;
        if (participant.Status != ParticipantStatus.UnpaidLate) return false;
        // TODO: Send email
        participant.CancelationNotified = true;
        db.SaveChanges();
        return true;
    }

    [Authorize(Roles = new[] { "Admin" })]
    public bool ForceCancelationStatus([Service] ApplicationDbContext db, int id, bool value)
    {
        var participant = db.Participants.Find(id);
        if (participant is null) return false;
        // TODO: Send email
        participant.CancelationNotified = value;
        db.SaveChanges();
        return true;
    }
}