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
    bool SendPaymentConfirmation(Participant participant);
    bool SendCancelConfirmation(Participant participant);
    bool SendForcedChangeConfirmation(Participant participant, bool status);
}