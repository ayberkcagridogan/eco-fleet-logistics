using EcoFleetLogistics.Application.Companies.Commands.CreateCompany;
using EcoFleetLogistics.Application.Companies.Commands.UpdateCompanyStatus;
using EcoFleetLogistics.Application.Companies.Queries.GetCompanies;
using EcoFleetLogistics.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EcoFleetLogistics.WebApi.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/company")
            .WithTags("Company")
            .RequireAuthorization(Policies.RequireSuperAdmin);

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
    }
}