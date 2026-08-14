var builder = DistributedApplication.CreateBuilder(args);

// ==========================================
// 1. INFRASTRUCTURE CONTAINERS
// ==========================================

var sqlPassword = builder.AddParameter("sql-password", secret: true);


var sqlserver = builder.AddSqlServer("sqlserver", password: sqlPassword, port: 1433)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();
    
var indentityDb = sqlserver.AddDatabase("identity-db");
var companyDb = sqlserver.AddDatabase("company-db");
var shipmentDb = sqlserver.AddDatabase("shipment-db");

var redis = builder.AddRedis("redis").WithRedisCommander();

var rabbitmq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

var seq = builder.AddSeq("seq", port: 5341);

// ==========================================
// 2. MICROSERVICES & REFERENCE BINDINGS
// ==========================================


// Identity API
var identityApi = builder.AddProject<Projects.EcoFleet_Identity_Api>("identity-api")
    .WithReference(indentityDb)
    .WaitFor(indentityDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq);

// Company API
var companyApi = builder.AddProject<Projects.EcoFleet_Company_Api>("company-api")
    .WithReference(companyDb)
    .WaitFor(companyDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq);

// Shipment API
var shipmentApi = builder.AddProject<Projects.EcoFleet_Shipment_Api>("shipment-api")
    .WithReference(shipmentDb)
    .WaitFor(shipmentDb)
    .WithReference(redis)
    .WithReference(rabbitmq)
    .WithReference(seq);

// 🚀 CENTRAL API GATEWAY (YARP)
var gatewayApi = builder.AddProject<Projects.EcoFleet_Gateway_Api>("gateway-api")
    .WithReference(identityApi)
    .WithReference(companyApi)
    .WithReference(shipmentApi)
    .WithReference(seq);

builder.Build().Run();
