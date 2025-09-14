using System.Net.Http.Headers;
using System.Text.Json;
using Facet.Mapping;
using RecipeApp.Models;
using RecipePortal.WebApp.Mappers;
using RecipePortal.WebApp.Models;

namespace RecipePortal.WebApp.Services;

/// <summary>
/// Service for handling recipe form operations including CRUD operations and AI analysis
/// </summary>
public class RecipeFormService : IRecipeFormService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecipeFormService> _logger;
    
    // Constants for configuration
    private const int MaxImageSizeBytes = 1 * 1024 * 1024; // 1MB
    private const int AITimeoutMinutes = 60; // 60 minutes (1 hour) for AI operations
    
    public RecipeFormService(HttpClient httpClient, ILogger<RecipeFormService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    
    public async Task<List<Category>> LoadCategoriesAsync()
    {
        try
        {
            _logger.LogInformation("Loading categories from API");
            
            var categories = await _httpClient.GetFromJsonAsync<List<Category>>("api/categories");
            
            if (categories == null)
            {
                _logger.LogWarning("API returned null for categories");
                return new List<Category>();
            }
            
            _logger.LogInformation("Successfully loaded {Count} categories", categories.Count);
            return categories;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading categories");
            throw new HttpRequestException($"Error loading categories: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading categories");
            throw new InvalidOperationException($"Unexpected error loading categories: {ex.Message}", ex);
        }
    }
    
    
    public async Task<PortalRecipe> LoadRecipeAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading recipe with ID: {RecipeId}", id);
            
            var recipe = await _httpClient.GetFromJsonAsync<Recipe>($"api/recipes/{id}");
            
            if (recipe == null)
            {
                _logger.LogWarning("Recipe not found with ID: {RecipeId}", id);
                throw new ArgumentException($"Recipe not found with ID: {id}", nameof(id));
            }
            
            // Convert Recipe to PortalRecipe using Facet mapping
            var portalRecipe = await recipe.ToFacetAsync<Recipe, PortalRecipe, RecipeMapper>();
            
            _logger.LogInformation("Successfully loaded recipe: {RecipeName}", recipe.Name.English);
            return portalRecipe;
        }
        catch (ArgumentException)
        {
            // Re-throw ArgumentException as-is
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while loading recipe {RecipeId}", id);
            throw new HttpRequestException($"Error loading recipe: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading recipe {RecipeId}", id);
            throw new InvalidOperationException($"Unexpected error loading recipe: {ex.Message}", ex);
        }
    }
    
    
    public async Task SaveRecipeAsync(PortalRecipe portalRecipe, Guid id)
    {
        try
        {
            _logger.LogInformation("Saving recipe with ID: {RecipeId}", id);
            
            // Convert PortalRecipe to Recipe using Facet mapping
            var recipe = await portalRecipe.ToFacetAsync<PortalRecipe, Recipe, PortalRecipeMapper>();
            
            HttpResponseMessage response;
            
            if (id == Guid.Empty)
            {
                _logger.LogInformation("Creating new recipe: {RecipeName}", recipe.Name.English);
                response = await _httpClient.PostAsJsonAsync("api/recipes", recipe);
            }
            else
            {
                _logger.LogInformation("Updating existing recipe: {RecipeName}", recipe.Name.English);
                response = await _httpClient.PutAsJsonAsync($"api/recipes/{id}", recipe);
            }
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully saved recipe");
                return;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to save recipe. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                throw new InvalidOperationException($"Failed to save recipe: {errorContent}");
            }
        }
        catch (InvalidOperationException)
        {
            // Re-throw InvalidOperationException as-is
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while saving recipe");
            throw new HttpRequestException($"Error saving recipe: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving recipe");
            throw new InvalidOperationException($"Unexpected error saving recipe: {ex.Message}", ex);
        }
    }
    
    
    public async Task<AIRecipeResponse> AnalyzeImageWithAIAsync(byte[] fileBytes, string contentType, string fileName)
    {
        try
        {
            _logger.LogInformation("Starting AI analysis for image: {FileName} ({FileSize} bytes)", fileName, fileBytes.Length);
            
            // Validate file size
            if (fileBytes.Length > MaxImageSizeBytes)
            {
                throw new ArgumentException($"Image file is too large. Maximum size is {MaxImageSizeBytes / (1024 * 1024)}MB", nameof(fileBytes));
            }
            
            // Create multipart content
            using var content = new MultipartFormDataContent();
            using var byteContent = new ByteArrayContent(fileBytes);
            
            byteContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(byteContent, "file", fileName);
            
            // Create cancellation token with extended timeout for AI operations
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(AITimeoutMinutes));
            
            var response = await _httpClient.PostAsync("api/ImageAI/analyse", content, cts.Token);
            
            if (response.IsSuccessStatusCode)
            {
                var jsonResult = await response.Content.ReadAsStringAsync();
                
                // Configure JSON serializer options to match API service configuration
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                };
                
                // Deserialize AI response
                var aiResponse = JsonSerializer.Deserialize<AIRecipeResponse>(jsonResult, jsonOptions);
                Console.WriteLine(jsonResult);
                if (aiResponse == null)
                {
                    _logger.LogWarning("AI response was null or invalid for image: {FileName}", fileName);
                    throw new InvalidOperationException("AI response was empty or invalid");
                }
                
                _logger.LogInformation("Successfully analyzed image: {FileName}", fileName);
                return aiResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("AI analysis failed for image: {FileName}. Status: {StatusCode}, Error: {Error}", 
                    fileName, response.StatusCode, errorContent);
                throw new InvalidOperationException($"AI analysis failed: {errorContent}");
            }
        }
        catch (ArgumentException)
        {
            // Re-throw ArgumentException as-is
            throw;
        }
        catch (InvalidOperationException)
        {
            // Re-throw InvalidOperationException as-is
            throw;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("AI analysis timed out for image: {FileName}", fileName);
            throw new TimeoutException($"AI analysis timed out after {AITimeoutMinutes} minutes. Please try again with a smaller image or simpler recipe.");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize AI response for image: {FileName}", fileName);
            throw new InvalidOperationException($"Error parsing AI response: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during AI analysis for image: {FileName}", fileName);
            throw new InvalidOperationException($"Error during AI analysis: {ex.Message}", ex);
        }
    }
}