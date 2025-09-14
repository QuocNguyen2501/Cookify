using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http;
using Microsoft.SemanticKernel;
using RecipeApp.ApiService.Data;
using RecipeApp.ApiService.Extensions;
using RecipeApp.ApiService.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddProblemDetails();

// Configure request timeout for AI operations (60 minutes)
builder.Services.AddRequestTimeouts(options =>
{
    // Add specific policy for AI analysis
    options.AddPolicy("AIAnalysisTimeout", new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
    {
        Timeout = TimeSpan.FromMinutes(60)
    });
});


// Configure HttpClient defaults for all services with extended timeout
builder.Services.ConfigureHttpClientDefaults(http =>
{
    http.ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(60); // Set 60-minute timeout for AI operations
    });
});

// Add Ollama chat completion service for recipe analysis
// builder.Services.AddOllamaForRecipeAnalysis();
builder.Services.AddAzureOpenAIForRecipeAnalysis(
    modelId: builder.Configuration["AzureOpenAI:ModelId"],
    endpoint: builder.Configuration["AzureOpenAI:Endpoint"],
    apiKey: builder.Configuration["AzureOpenAI:ApiKey"]
    );

// Also configure any named HttpClient that might be used by other services
builder.Services.Configure<HttpClientFactoryOptions>(options =>
{
    options.HttpClientActions.Add(client =>
    {
        client.Timeout = TimeSpan.FromMinutes(60);
    });
});


builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddSingleton<KernelPluginCollection>((serviceProvider)=> []);
builder.Services.AddTransient((serviceProvider) =>
{
    KernelPluginCollection plugins = serviceProvider.GetRequiredService<KernelPluginCollection>();
    return new Kernel(serviceProvider, plugins);
});

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("recipesdb"), 
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

// Register business services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();
builder.Services.AddScoped<IRecipeAIAnalysisService, AzureOpenAIRecipeAIAnalysisService>();

// Add API services with JSON options to handle circular references
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references by ignoring cycles
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add OpenAPI/Swagger support
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
// Add request timeout middleware before other middleware
app.UseRequestTimeouts();

// Add Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Seed database with sample data
await DatabaseSeeder.SeedDataAsync(app);

// Map controllers
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
