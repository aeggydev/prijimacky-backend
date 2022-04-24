using prijimacky_backend.DTO;
using prijimacky_backend.Entities;
using prijimacky_backend.Graphql.Types;

namespace prijimacky_backend.Services;

public interface IAdminService
{
    string LoginToken(LoginInfo login);
    int CreateAdmin(RegistrationInfo info);
    int DeleteAdmin(int id);
}

public interface IParticipantService
{
    IEnumerable<Participant> GetAll();
    Participant? GetById(int id);
    
    Participant Create(NewParticipant newParticipant, string ip);
    Participant Update(int id, UpdateParticipant input);
    IEnumerable<Participant> UpdateMultiple(IEnumerable<UpdateParticipantsItem> inputs);
    bool Delete(int id);

    bool ConfirmPayment(int id);
    bool ConfirmLateCancel(int id);
    bool ForceCancel(int id, bool status);
}
public interface ISettingsService
{
    Settings Get();
    Settings Update(UpdateSettings input);
}
public interface IStatisticsService
{
    Statistics Get();
}
public interface IEmailService
{
    EmailStatistics GetStatistics();
    Task<bool> SendPaymentConfirmation(Participant participant);
    Task<bool> SendCancelConfirmation(Participant participant);
    Task<bool> SendForcedChangeConfirmation(Participant participant, bool status);
    // emails: https://docs.google.com/document/d/1VzHYK9jW2UXz3cQ49GuLBkihUW_k1osU738rdpLtDdY/edit?usp=sharing
}