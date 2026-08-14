using System.Threading.RateLimiting;
using EcoFleet.Shared.Kernel;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Aspire Service Defaults & Service Discovery
builder.Services.AddSharedKernel(builder);

// 1. Centralized CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("GatewayCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Centralized Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed-window", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100;
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// 3. YARP Reverse Proxy & Service Discovery Integration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver(); // Microsoft.Extensions.ServiceDiscovery.Yarp paketinden gelir

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseSharedKernelMiddlewares();

app.UseCors("GatewayCorsPolicy");
app.UseRateLimiter();

app.MapReverseProxy();

app.Run();