using Microsoft.EntityFrameworkCore;
using RecipeApp.ApiService.Data;
using RecipeApp.ApiService.Extensions;
using RecipeApp.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

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

// Add API services
builder.Services.AddControllers();

// Add OpenAPI/Swagger support
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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
