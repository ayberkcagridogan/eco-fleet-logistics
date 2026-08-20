var builder = DistributedApplication.CreateBuilder(args);

// ==========================================
// 1. INFRASTRUCTURE CONTAINERS
// ==========================================

//var sqlserver = builder.AddConnectionString("sqlserver");

var identityDb = builder.AddConnectionString("identity-db");
var companyDb = builder.AddConnectionString("company-db");
var shipmentDb = builder.AddConnectionString("shipment-db");

var redis = builder.AddRedis("redis").WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

var seq = builder.AddSeq("seq", port: 5341);

// ==========================================
// 2. MICROSERVICES & REFERENCE BINDINGS
// ==========================================

var jwtSecret = builder.Configuration["JwtSettings:Secret"] 
    ?? throw new InvalidOperationException("JwtSettings:Secret missing in AppHost configuration!");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException("JwtSettings:Issuer missing in AppHost configuration!");
var jwtAudience = builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException("JwtSettings:Audience missing in AppHost configuration!");
var jwtExpiryMinutes = builder.Configuration["JwtSettings:ExpiryMinutes"]
    ?? throw new InvalidOperationException("JwtSettings:ExpiryMinutes missing in AppHost configuration!");

// Company API
var companyApi = builder.AddProject<Projects.EcoFleet_Company_Api>("company-api")
    .WithReference(companyDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq)
    .WithHttpEndpoint(name: "grpc")
    .WithEnvironment("JwtSettings__Secret" , jwtSecret)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__ExpiryMinutes", jwtExpiryMinutes);

// Identity API
var identityApi = builder.AddProject<Projects.EcoFleet_Identity_Api>("identity-api")
    .WithReference(identityDb)
    .WithReference(companyApi.GetEndpoint("grpc"))
    .WithReference(companyApi)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq) 
    .WithEnvironment("JwtSettings__Secret" , jwtSecret)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__ExpiryMinutes", jwtExpiryMinutes);


// Shipment API
var shipmentApi = builder.AddProject<Projects.EcoFleet_Shipment_Api>("shipment-api")
    .WithReference(shipmentDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq)
    .WithEnvironment("JwtSettings__Secret" , jwtSecret)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__ExpiryMinutes", jwtExpiryMinutes);


// 🚀 CENTRAL API GATEWAY (YARP)
var gatewayApi = builder.AddProject<Projects.EcoFleet_Gateway_Api>("gateway-api")
    .WithEnvironment("JwtSettings__Secret" , jwtSecret)
    .WithEnvironment("JwtSettings__Issuer", jwtIssuer)
    .WithEnvironment("JwtSettings__Audience", jwtAudience)
    .WithEnvironment("JwtSettings__ExpiryMinutes", jwtExpiryMinutes)
    .WithReference(identityApi)
    .WithReference(companyApi)
    .WithReference(shipmentApi)
    .WithReference(seq);

builder.Build().Run();
