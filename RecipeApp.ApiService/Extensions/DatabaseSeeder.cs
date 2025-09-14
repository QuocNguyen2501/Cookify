using Microsoft.EntityFrameworkCore;
using RecipeApp.ApiService.Data;
using RecipeApp.Models;

namespace RecipeApp.ApiService.Extensions;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        try
        {
            // Apply pending migrations
            await context.Database.MigrateAsync();
        }
        catch (Exception ex) when (ex.Message.Contains("There is already an object named"))
        {
            // Handle case where tables exist but migration history is missing
            // This can happen when switching from EnsureCreatedAsync to migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            
            // If we have pending migrations but the base tables already exist, 
            // mark the initial migration as applied
            if (pendingMigrations.Any() && !appliedMigrations.Any())
            {
                var initialMigrations = pendingMigrations.Where(m => m.Contains("InitialCreate")).ToList();
                
                // Mark initial migrations as applied by inserting into migration history
                foreach (var migration in initialMigrations)
                {
                    await context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, {1})",
                        migration, "9.0.0");
                }
                
                // Now apply remaining migrations
                await context.Database.MigrateAsync();
            }
        }
        
        // Check if data already exists
        if (await context.Categories.AnyAsync())
            return;
        
        // Seed Categories
        var categories = new[]
        {
            new Category("Appetizers", "Món khai vị") { ImageFileName = "appetizers.jpg" },
            new Category("Main Courses", "Món chính") { ImageFileName = "main-courses.jpg" },
            new Category("Desserts", "Món tráng miệng") { ImageFileName = "desserts.jpg" },
            new Category("Beverages", "Đồ uống") { ImageFileName = "beverages.jpg" }
        };
        
        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }
}
