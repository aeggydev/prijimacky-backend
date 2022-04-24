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
using prijimacky_backend.Services;

namespace prijimacky_backend.Graphql;

public class Query
{
    public string Login([Service] IAdminService adminService, LoginInfo login) => adminService.LoginToken(login);

    [Authorize(Roles = new[] { "Admin" })]
    public IEnumerable<Participant> GetParticipants([Service] IParticipantService participantService) =>
        participantService.GetAll();

    public Settings GetSettings([Service] ISettingsService settings) => settings.Get();

    public Statistics GetStatistics([Service] IStatisticsService statisticsService) => statisticsService.Get();

    public EmailStatistics GetEmailStatistics([Service] IEmailService emailService) => emailService.GetStatistics();
}

public class Mutation
{
    public Participant AddParticipant([Service] IParticipantService participantService,
        [Service] IHttpContextAccessor httpContextAccessor, NewParticipant newParticipant)
    {
        var ip = httpContextAccessor.HttpContext!.Connection.RemoteIpAddress!.ToString();
        return participantService.Create(newParticipant, ip);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Participant UpdateParticipant([Service] IParticipantService participantService,
        int id, UpdateParticipant updateParticipant) =>
        participantService.Update(id, updateParticipant);

    [Authorize(Roles = new[] { "Admin" })]
    public IEnumerable<Participant> UpdateParticipants([Service] IParticipantService participantService,
        IEnumerable<UpdateParticipantsItem> updateParticipants)
    {
        return participantService.UpdateMultiple(updateParticipants);
    }

    [Authorize(Roles = new[] { "Admin" })]
    public Settings UpdateSettings([Service] ISettingsService settings, UpdateSettings updateSettings) =>
        settings.Update(updateSettings);

    [Authorize(Roles = new[] { "Admin" })]
    public int AddAdmin([Service] IAdminService adminService, RegistrationInfo login) =>
        adminService.CreateAdmin(login);

    [Authorize(Roles = new[] { "Admin" })]
    public bool RemoveParticipant([Service] IParticipantService participantService, int id) =>
        participantService.Delete(id);

    [Authorize(Roles = new[] { "Admin" })]
    public bool ConfirmPayment([Service] IParticipantService participantService, int id) =>
        participantService.ConfirmPayment(id);


    [Authorize(Roles = new[] { "Admin" })]
    public bool ConfirmLateCancel([Service] IParticipantService participantService, int id) =>
        participantService.ConfirmLateCancel(id);

    [Authorize(Roles = new[] { "Admin" })]
    public bool ForceCancelationStatus([Service] IParticipantService participantService, int id, bool value) =>
        participantService.ForceCancel(id, value);
}