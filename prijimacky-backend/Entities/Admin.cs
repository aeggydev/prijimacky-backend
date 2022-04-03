using System.ComponentModel.DataAnnotations;

namespace prijimacky_backend.Entities;

public class Admin
{
    [Required] public int Id { get; set; }
    [Required] public string Username { get; set; } = default!;
    
    // BCrypt password hash with integrated salt
    [Required] public string PasswordHash { get; set; } = default!;

    public bool CheckPassword(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}