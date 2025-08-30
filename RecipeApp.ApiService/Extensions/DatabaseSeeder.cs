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
        
        // Seed Recipes
        var recipes = new[]
        {
            new Recipe
            {
                CategoryId = categories[0].Id, // Appetizers
                Name = new RecipeLocalizedText("Spring Rolls", "Chả giò"),
                Description = new RecipeLocalizedText("Crispy Vietnamese spring rolls with fresh vegetables", "Chả giò giòn rụm với rau củ tươi ngon"),
                PrepTime = "10 minutes",
                CookTime = "10 minutes",
                ImageFileName = "spring-rolls.jpg",
                Ingredients = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Rice paper", "Bánh tráng"),
                    new RecipeLocalizedText("Lettuce", "Xà lách"),
                    new RecipeLocalizedText("Carrot", "Cà rốt"),
                    new RecipeLocalizedText("Cucumber", "Dưa chuột"),
                    new RecipeLocalizedText("Mint leaves", "Lá bạc hà"),
                    new RecipeLocalizedText("Dipping sauce", "Nước chấm")
                },
                Instructions = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Soak rice paper in warm water", "Nhúng bánh tráng vào nước ấm"),
                    new RecipeLocalizedText("Add lettuce and vegetables", "Thêm xà lách và rau củ"),
                    new RecipeLocalizedText("Roll tightly", "Cuốn chặt"),
                    new RecipeLocalizedText("Serve with dipping sauce", "Ăn kèm với nước chấm")
                }
            },
            new Recipe
            {
                CategoryId = categories[1].Id, // Main Courses
                Name = new RecipeLocalizedText("Beef Pho", "Phở bò"),
                Description = new RecipeLocalizedText("Traditional Vietnamese beef noodle soup", "Món phở bò truyền thống Việt Nam"),
                PrepTime = "30 minutes",
                CookTime = "6 hours",
                ImageFileName = "beef-pho.jpg",
                Ingredients = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Beef bones", "Xương bò"),
                    new RecipeLocalizedText("Rice noodles", "Bánh phở"),
                    new RecipeLocalizedText("Beef slices", "Thịt bò thái lát"),
                    new RecipeLocalizedText("Onion", "Hành tây"),
                    new RecipeLocalizedText("Star anise", "Hoa hồi"),
                    new RecipeLocalizedText("Cinnamon", "Quế"),
                    new RecipeLocalizedText("Fish sauce", "Nước mắm"),
                    new RecipeLocalizedText("Bean sprouts", "Giá đỗ"),
                    new RecipeLocalizedText("Herbs", "Rau thơm")
                },
                Instructions = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Simmer beef bones for 6 hours", "Ninh xương bò trong 6 tiếng"),
                    new RecipeLocalizedText("Add spices and seasonings", "Thêm gia vị và nêm nếm"),
                    new RecipeLocalizedText("Cook rice noodles separately", "Luộc bánh phở riêng"),
                    new RecipeLocalizedText("Assemble bowl with noodles and beef", "Xếp bánh phở và thịt bò vào tô"),
                    new RecipeLocalizedText("Pour hot broth over", "Rót nước dúng nóng"),
                    new RecipeLocalizedText("Garnish with herbs and bean sprouts", "Ăn kèm rau thơm và giá đỗ")
                }
            },
            new Recipe
            {
                CategoryId = categories[2].Id, // Desserts
                Name = new RecipeLocalizedText("Mango Sticky Rice", "Xôi xoài"),
                Description = new RecipeLocalizedText("Sweet sticky rice with fresh mango and coconut milk", "Xôi ngọt với xoài tươi và nước cốt dừa"),
                PrepTime = "15 minutes",
                CookTime = "30 minutes",
                ImageFileName = "mango-sticky-rice.jpg",
                Ingredients = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Glutinous rice", "Gạo nếp"),
                    new RecipeLocalizedText("Coconut milk", "Nước cốt dừa"),
                    new RecipeLocalizedText("Sugar", "Đường"),
                    new RecipeLocalizedText("Salt", "Muối"),
                    new RecipeLocalizedText("Fresh mango", "Xoài tươi"),
                    new RecipeLocalizedText("Toasted sesame seeds", "Mè rang")
                },
                Instructions = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Soak glutinous rice overnight", "Ngâm gạo nếp qua đêm"),
                    new RecipeLocalizedText("Steam rice until tender", "Hấp gạo cho đến khi mềm"),
                    new RecipeLocalizedText("Mix coconut milk with sugar and salt", "Pha nước cốt dừa với đường và muối"),
                    new RecipeLocalizedText("Pour over steamed rice", "Rưới lên xôi"),
                    new RecipeLocalizedText("Serve with sliced mango", "Ăn kèm với xoài thái lát"),
                    new RecipeLocalizedText("Sprinkle with sesame seeds", "Rắc mè rang lên trên")
                }
            },
            new Recipe
            {
                CategoryId = categories[3].Id, // Beverages
                Name = new RecipeLocalizedText("Vietnamese Iced Coffee", "Cà phê sữa đá"),
                Description = new RecipeLocalizedText("Strong coffee with sweetened condensed milk over ice", "Cà phê đậm đà với sữa đặc có đường và đá"),
                PrepTime = "5 minutes",
                CookTime = "5 minutes",
                ImageFileName = "vietnamese-iced-coffee.jpg",
                Ingredients = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Vietnamese coffee grounds", "Bột cà phê Việt Nam"),
                    new RecipeLocalizedText("Sweetened condensed milk", "Sữa đặc có đường"),
                    new RecipeLocalizedText("Hot water", "Nước nóng"),
                    new RecipeLocalizedText("Ice cubes", "Đá viên")
                },
                Instructions = new List<RecipeLocalizedText>
                {
                    new RecipeLocalizedText("Add condensed milk to glass", "Cho sữa đặc vào ly"),
                    new RecipeLocalizedText("Place Vietnamese coffee filter on top", "Đặt phin cà phê lên trên"),
                    new RecipeLocalizedText("Add coffee grounds to filter", "Cho bột cà phê vào phin"),
                    new RecipeLocalizedText("Pour hot water slowly", "Rót nước nóng từ từ"),
                    new RecipeLocalizedText("Let coffee drip", "Chờ cà phê nhỏ giọt"),
                    new RecipeLocalizedText("Stir and add ice", "Khuấy đều và cho đá")
                }
            }
        };
        
        await context.Recipes.AddRangeAsync(recipes);
        await context.SaveChangesAsync();
    }
}
