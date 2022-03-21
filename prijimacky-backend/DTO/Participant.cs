namespace prijimacky_backend.DTO;

public record NewParticipant(
    string ParticipantName,
    string ParticipantSurname,
    string ParentName,
    string ParentSurname,
    string School,
    string Phone,
    string Email);
