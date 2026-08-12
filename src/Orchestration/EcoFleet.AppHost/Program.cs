var builder = DistributedApplication.CreateBuilder(args);

var identityApi = builder.AddProject<Projects.EcoFleet_Identity_Api>("identity-api");

var companyApi = builder.AddProject<Projects.EcoFleet_Company_Api>("company-api");

var shipmentApi = builder.AddProject<Projects.EcoFleet_Shipment_Api>("shipment-api");

builder.Build().Run();
