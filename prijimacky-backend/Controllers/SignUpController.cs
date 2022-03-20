using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using prijimacky_backend.Data;

namespace prijimacky_backend.Controllers;

public record NewParticipant(
    string ParticipantName,
    string ParticipantSurname,
    string ParentName,
    string ParentSurname,
    string School,
    string Phone,
    string Email);

[ApiController]
[Route("[controller]")]
public class SignUpController
{
    private readonly ApplicationDbContext _dbContext;

    public SignUpController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public NewParticipant SignUp(NewParticipant newParticipant)
    {
        Console.WriteLine(JsonSerializer.Serialize(newParticipant));
        return newParticipant;
    }
}