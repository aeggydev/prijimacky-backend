using prijimacky_backend.Data;
using prijimacky_backend.Graphql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();
builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

// Make this less specific
app.UseCors(opt => opt.AllowAnyHeader().AllowAnyOrigin().AllowAnyHeader());

app.MapControllers();

app.MapGraphQL();

app.Run();