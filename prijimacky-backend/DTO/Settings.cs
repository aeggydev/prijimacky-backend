namespace prijimacky_backend.DTO;

public record UpdateSettings(
    bool SignUpAllowed, int Capacity, int AllowedOver);