using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using prijimacky_backend.Data;
using prijimacky_backend.DTO;
using prijimacky_backend.Entities;

namespace prijimacky_backend.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;

    public AdminService(ApplicationDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public string LoginToken(LoginInfo login)
    {
        var (username, password) = login;
        var admin = _db.Admins
            .FirstOrDefault(a => a.Username == username);
        if (admin is null) throw new Exception("Invalid username");

        if (!admin.CheckPassword(password)) throw new Exception("Invalid password");

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
        var authClaims = new List<Claim>
        {
            new("sub", username),
            new(ClaimTypes.Role, "Admin")
        };
        var token = new JwtSecurityToken(
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            expires: DateTime.Now.AddDays(14),
            claims: authClaims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int CreateAdmin(RegistrationInfo info)
    {
        if (_db.Admins.Any(x => x.Username == info.Username))
            throw new Exception("Username already exists");

        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var hash = BCrypt.Net.BCrypt.HashPassword(info.Password, salt);
        var admin = new Admin
        {
            PasswordHash = hash,
            Username = info.Username
        };
        _db.Admins.Add(admin);
        _db.SaveChanges();
        return admin.Id;
    }

    public int DeleteAdmin(int id)
    {
        throw new NotImplementedException();
    }
}