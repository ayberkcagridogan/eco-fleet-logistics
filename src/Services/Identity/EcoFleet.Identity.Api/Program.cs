using EcoFleet.Identity.Api.Endpoints;
using EcoFleet.Identity.Api.Extensions;
using EcoFleet.Identity.Application;
using EcoFleet.Identity.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityAuthorizationPolicies();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAutEndPoints();
app.MapUsersEndpoints();

app.Run();