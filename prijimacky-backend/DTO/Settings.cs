namespace prijimacky_backend.DTO;

public class UpdateSettings
{
    public bool? SignUpAllowed { get; set; }
    public int? Capacity { get; set; }
    
    // "Povolená místa pod čarou"
    public int? AllowedOver { get; set; }
}