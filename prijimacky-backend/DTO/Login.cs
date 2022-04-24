namespace prijimacky_backend.DTO;

public record LoginInfo(string Username, string Password);
public record RegistrationInfo(string Username, string Password, string Email);