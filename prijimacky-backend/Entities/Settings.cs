namespace prijimacky_backend.Entities;

public class Settings
{
    public int Id { get; set; }
    public bool SignUpAllowed { get; set; } = true;
    public int Capacity { get; set; } = 100;
    
    // "Povolená místa pod čarou"
    public int AllowedOver { get; set; } = 10;
}