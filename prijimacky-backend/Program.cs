using System.Globalization;
using System.Text;
using CsvHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using prijimacky_backend.Data;
using prijimacky_backend.Graphql;
using prijimacky_backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddErrorFilter<GraphQLErrorFilter>()
    .AddAuthorization();
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateAudience = false,
        ValidateIssuer = false
    };
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

using (var serviceScope = app.Services.CreateScope())
{
    var dbService = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
    if (dbService is null) throw new Exception();

    // Seed database (for settings)
    // TODO: Replace this with migrations when moving away from InMemoryDatabase
    dbService.Database.EnsureCreated();
}

app.MapGet("/file/table.csv", [Authorize(Roles = "Admin")]([Service] ApplicationDbContext db) =>
{
    // TODO: Use czech header cells

    using MemoryStream memoryStream = new();
    using StreamWriter writer = new(memoryStream, Encoding.UTF8);
    using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
    csv.WriteRecords(db.Participants);
    return Results.File(memoryStream.GetBuffer(), "text/csv", "table.csv");
});
// TODO: Add endpoint for signing up without auth

app.UseHttpsRedirection();

// Make this less specific
app.UseCors(opt => opt.AllowAnyHeader().AllowAnyOrigin().AllowAnyHeader());

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGraphQL();

app.Run();