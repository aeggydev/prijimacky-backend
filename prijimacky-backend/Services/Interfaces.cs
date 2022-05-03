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

    Task<bool> StatusAction(int id, ParticipantStatus presumedStatus);
    Task<bool> StatusActionAllOfStatus(ParticipantStatus expectedStatus);
    // handle status, presumedStatus specifies the status that the frontend thinks is currently set 
    // returns false if presumedStatus doesn't patch

    bool ClearPaid(int id);
    // clears the paid property of the participant
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
    Task<bool> StatusAction(Participant participant, ParticipantStatus presumedStatus);
    // not implemented for now: Task<bool> StatusActionAllOfStatus(ParticipantStatus expectedStatus);
    // emails: https://docs.google.com/document/d/1VzHYK9jW2UXz3cQ49GuLBkihUW_k1osU738rdpLtDdY/edit?usp=sharing
}