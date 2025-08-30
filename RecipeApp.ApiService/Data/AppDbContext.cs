using Microsoft.EntityFrameworkCore;
using RecipeApp.Models;
using System.Text.Json;

namespace RecipeApp.ApiService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure RecipeLocalizedText as JSON for Category.Name
        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<RecipeLocalizedText>(v, (JsonSerializerOptions?)null) ?? new RecipeLocalizedText()
            );

        // Configure RecipeLocalizedText as JSON for Recipe.Name
        modelBuilder.Entity<Recipe>()
            .Property(r => r.Name)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<RecipeLocalizedText>(v, (JsonSerializerOptions?)null) ?? new RecipeLocalizedText()
            );

        // Configure RecipeLocalizedText as JSON for Recipe.Description
        modelBuilder.Entity<Recipe>()
            .Property(r => r.Description)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<RecipeLocalizedText>(v, (JsonSerializerOptions?)null) ?? new RecipeLocalizedText()
            );

        // Configure List<RecipeLocalizedText> as JSON for Recipe.Ingredients
        modelBuilder.Entity<Recipe>()
            .Property(r => r.Ingredients)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<RecipeLocalizedText>>(v, (JsonSerializerOptions?)null) ?? new List<RecipeLocalizedText>()
            )
            .Metadata.SetValueComparer(new ListRecipeLocalizedTextValueComparer());

        // Configure List<RecipeLocalizedText> as JSON for Recipe.Instructions
        modelBuilder.Entity<Recipe>()
            .Property(r => r.Instructions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<RecipeLocalizedText>>(v, (JsonSerializerOptions?)null) ?? new List<RecipeLocalizedText>()
            )
            .Metadata.SetValueComparer(new ListRecipeLocalizedTextValueComparer());

        // Configure one-to-many relationship between Category and Recipe
        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes

        // Configure unique index for Category names (optional but recommended)
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}

// Custom value comparer for List<RecipeLocalizedText> to enable proper change tracking
public class ListRecipeLocalizedTextValueComparer : Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<RecipeLocalizedText>>
{
    public ListRecipeLocalizedTextValueComparer() : base(
        (c1, c2) => CompareList(c1, c2),
        c => ComputeHashCode(c),
        c => c.ToList())
    {
    }

    private static bool CompareList(List<RecipeLocalizedText>? list1, List<RecipeLocalizedText>? list2)
    {
        if (list1 == null && list2 == null) return true;
        if (list1 == null || list2 == null) return false;
        return list1.SequenceEqual(list2);
    }

    private static int ComputeHashCode(List<RecipeLocalizedText> list)
    {
        return list.Aggregate(0, (a, v) => HashCode.Combine(a, v.English?.GetHashCode() ?? 0, v.Vietnamese?.GetHashCode() ?? 0));
    }
}
