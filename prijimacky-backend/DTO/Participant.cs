namespace prijimacky_backend.DTO;

public record NewParticipant(
    string ParticipantName,
    string ParticipantSurname,
    string ParentName,
    string ParentSurname,
    string School,
    string Phone,
    string Email);

public record UpdateParticipant(
    string? ParticipantName,
    string? ParticipantSurname,
    string? ParentName,
    string? ParentSurname,
    string? School,
    string? Phone,
    string? Email,
    DateOnly? DueDate,
    DateOnly? PaidDate);

// TODO: Used for batch updating
public record UpdateParticipantsItem(
    int Id,
    UpdateParticipant data);