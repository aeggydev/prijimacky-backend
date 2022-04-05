using CsvHelper.Configuration.Attributes;

namespace prijimacky_backend.Entities;

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
}