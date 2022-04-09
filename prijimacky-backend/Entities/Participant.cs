using CsvHelper.Configuration.Attributes;

namespace prijimacky_backend.Entities;

public enum ParticipantStatus
{
    NotNotified, // Not notified about being registered
    Unpaid, // Waiting for payment
    UnpaidLate, // Over DueDate, waiting to be notified about cancellation
    Canceled, // Canceled or unpaid
    PaidUnconfirmed, // Paid, waiting to be notified about confirmation
    PaidConfirmed, // Paid and notified about confirmation
    Error // Something went wrong
}

public class Participant
{
    // TODO: Add admin-facing ID
    [Name("Id")] public int Id { get; set; }
    [Name("Jméno účastníka")] public string ParticipantName { get; set; } = "";
    [Name("Příjmení účastníka")] public string ParticipantSurname { get; set; } = "";
    [Name("Jméno zákonného zástupce")] public string ParentName { get; set; } = "";
    [Name("Příjmení zákonného zástupce")] public string ParentSurname { get; set; } = "";
    [Name("Škola")] public string School { get; set; } = "";
    [Name("Telefonní číslo")] public string Phone { get; set; } = "";
    [Name("E-mailová adresa")] public string Email { get; set; } = "";
    [Name("IP adresa")] public string Ip { get; set; } = "";
    [Name("Variabilní symbol")] public string VariableSymbol { get; set; } = "";
    [Name("Datum přihlášení")] public DateTime SignUpDate { get; set; }
    [Name("Datum splatnosti")] public DateTime DueDate => SignUpDate.AddDays(15);
    [Name("Datum uhrazení")] public DateTime? PaidDate { get; set; }

    public bool IsPaid => PaidDate != null;
    public bool IsOver => !IsPaid && DateTime.Now > DueDate;

    // Notified about creation and with payment info
    public bool CreationNotified { get; set; } = false;

    // Notified that participant was canceled due to lack of payment
    public bool CancelationNotified { get; set; } = false;

    // Notified that payment received and confirmed
    public bool PaidNotified { get; set; } = false;

    public ParticipantStatus Status => this switch
    {
        { CreationNotified: false } => ParticipantStatus.NotNotified,
        { PaidNotified: true, IsOver: true } => ParticipantStatus.Error,
        { CreationNotified: true, IsPaid: false, IsOver: false } => ParticipantStatus.Unpaid,
        { CreationNotified: true, IsOver: true, CancelationNotified: false } => ParticipantStatus.UnpaidLate,
        { CreationNotified: true, IsOver: true, CancelationNotified: true } => ParticipantStatus.Canceled,
        { CreationNotified: true, IsPaid: true, PaidNotified: false } => ParticipantStatus.PaidUnconfirmed,
        { CreationNotified: true, IsPaid: true, PaidNotified: true } => ParticipantStatus.PaidConfirmed,
        _ => ParticipantStatus.Error
    };
}