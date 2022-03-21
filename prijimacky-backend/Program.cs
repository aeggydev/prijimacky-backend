using prijimacky_backend.Data;
using prijimacky_backend.Graphql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>();
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();