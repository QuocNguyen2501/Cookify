var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.RecipeApp_ApiService>("apiservice");

builder.AddProject<Projects.RecipePortal_WebApp>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
