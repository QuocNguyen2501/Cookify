namespace RecipeApp.ApiService.Services;

/// <summary>
/// Interface for AI-powered recipe analysis from extracted text
/// </summary>
public interface IRecipeAIAnalysisService
{
    /// <summary>
    /// Analyzes extracted text from an image to generate structured recipe data
    /// </summary>
    /// <param name="extractedText">Text extracted from recipe image using OCR</param>
    /// <returns>JSON string containing structured recipe data with English and Vietnamese content</returns>
    Task<string> AnalyzeRecipeFromTextAsync(string extractedText);


    /// <summary>
    /// Gets the name of the AI provider being used
    /// </summary>
    /// <returns>Provider name (e.g., "Ollama", "OpenAI", "Azure AI")</returns>
    string GetProviderName();
}