using EcoFleet.Identity.Api.Endpoints;
using EcoFleet.Identity.Api.Extensions;
using EcoFleet.Identity.Application;
using EcoFleet.Identity.Infrastructure;
using EcoFleet.Shared.Kernel;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedKernel(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityAuthorizationPolicies();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "identity-db-check",
        tags: new[] { "db", "ready" });

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health").AllowAnonymous();;
app.UseHttpsRedirection();

app.MapAutEndPoints();
app.MapUsersEndpoints();


app.Run();