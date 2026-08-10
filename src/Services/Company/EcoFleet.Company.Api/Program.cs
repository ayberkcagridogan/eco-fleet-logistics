using EcoFleet.Company.Application;
using EcoFleet.Company.Application.Companies.Commands.CreateCompany;
using EcoFleet.Company.Application.Companies.Commands.UpdateCompanyStatus;
using EcoFleet.Company.Application.Companies.Queries.GetCompanies;
using EcoFleet.Company.Infrastructure;
using EcoFleet.Shared.Kernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSharedKernel(builder.Configuration);
builder.Services.AddCompanyInfrastructure(builder.Configuration);
builder.Services.AddCompanyApplication();
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks()
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
        name: "company-db-check",
        tags: new[] { "db", "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health").AllowAnonymous();
app.UseHttpsRedirection();

var group = app.MapGroup("/api/v1/company")
            .WithTags("Company");
            //.RequireAuthorization(Policies.RequireSuperAdmin);

        // 1. Create Company
        group.MapPost("/", async (
            [FromBody] CreateCompanyCommand command, 
            IMediator mediator) =>
        {
            var companyId = await mediator.Send(command);
            return Results.Created($"/api/v1/companies/{companyId}", new { Id = companyId });
        })
        .WithName("CreateCompany")
        .WithOpenApi();

        // 2. Update Company Status (Toggle Active/Inactive)
        group.MapPatch("/{id:guid}/status", async (
            Guid id, 
            [FromBody] UpdateStatusRequest request, 
            IMediator mediator) =>
        {
            var command = new UpdateCompanyStatusCommand(id, request.IsActive);
            var result = await mediator.Send(command);

            return result 
                ? Results.NoContent() 
                : Results.NotFound(new { Message = $"Company with ID '{id}' was not found." });
        })
        .WithName("UpdateCompanyStatus")
        .WithOpenApi();

        // 3. Get Companies with Pagination & Filtering
        group.MapGet("/", async (
            [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string? searchTerm,
            [FromQuery] bool? isActive,
            IMediator mediator) =>
        {
            var query = new GetCompaniesQuery(
                page <= 0 ? 1 : page, 
                pageSize <= 0 ? 10 : pageSize, 
                searchTerm, 
                isActive);

            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCompanies")
        .WithOpenApi();


app.Run();

