using RecipeApp.ApiService.Models;

namespace RecipeApp.ApiService.Models.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public RecipeLocalizedText Name { get; set; } = new();
        
        public static CategoryDto FromEntity(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
