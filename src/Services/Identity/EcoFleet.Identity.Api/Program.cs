using EcoFleet.Identity.Api.Endpoints;
using EcoFleet.Identity.Api.Extensions;
using EcoFleet.Identity.Application;
using EcoFleet.Identity.Infrastructure;
using EcoFleet.Identity.Infrastructure.Persistence;
using EcoFleet.Shared.Kernel;
using EcoFleet.Shared.Kernel.Grpc;
using EcoFleet.Shared.Kernel.Persistence.Extensions;


AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedKernel(builder);
builder.Services.AddIdentityAuthorizationPolicies();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();

builder.Services.AddGrpcClient<CompanyGrpcService.CompanyGrpcServiceClient>((sp, options) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    
    var companyApiUrl = config["services:company-api:grpc:0"] 
                     ?? config["services:company-api:http:0"]
                     ?? throw new InvalidOperationException("company-api endpoint configuration not found!");

    options.Address = new Uri(companyApiUrl);
});

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
    await app.ApplyMigrationsAndSeedAsync<IdentityDbContext>();
}

app.UseHttpsRedirection();
app.UseSharedKernelMiddlewares();

app.MapAutEndPoints();
app.MapUsersEndpoints();
app.UseSharedKernelEndpoints();

await app.Services.SeedIdentityDatabaseAsync();
app.Run();