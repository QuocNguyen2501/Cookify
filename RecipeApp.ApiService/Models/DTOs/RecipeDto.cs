using RecipeApp.Models;

namespace RecipeApp.ApiService.Models.DTOs
{
    public class RecipeDto
    {
        public Guid Id { get; set; }
        public RecipeLocalizedText Name { get; set; } = new();
        public RecipeLocalizedText Description { get; set; } = new();
        public string PrepTime { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string? ImageFileName { get; set; }
        public List<RecipeLocalizedText> Ingredients { get; set; } = [];
        public List<RecipeLocalizedText> Instructions { get; set; } = [];

        public Guid CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
        
        public static RecipeDto FromEntity(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                PrepTime = recipe.PrepTime,
                CookTime = recipe.CookTime,
                ImageFileName = recipe.ImageFileName,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                CategoryId = recipe.Category!.Id,
                Category = recipe.Category != null ? CategoryDto.FromEntity(recipe.Category) : null
            };
        }
    }
}
