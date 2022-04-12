namespace prijimacky_backend.Entities;

public class Settings
{
    public int Id { get; set; }
    public bool SignUpAllowed { get; set; } = true;
    public DateOnly SignUpUntil { get; set; } = DateOnly.FromDateTime(DateTime.Now).AddMonths(1);
    public DateOnly SignUpFrom { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public int Capacity { get; set; } = 100;
    
    // "Povolená místa pod čarou"
    public int AllowedOver { get; set; } = 10;
}