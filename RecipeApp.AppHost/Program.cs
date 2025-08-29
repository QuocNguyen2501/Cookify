var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server database
var sqlServer = builder.AddSqlServer("sqlserver")
                      .WithDataVolume();

var database = sqlServer.AddDatabase("recipesdb");

var apiService = builder.AddProject<Projects.RecipeApp_ApiService>("apiservice")
                       .WithReference(database);

builder.AddProject<Projects.RecipePortal_WebApp>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
