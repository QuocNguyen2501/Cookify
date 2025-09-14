using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RecipeApp.ApiService.Constants;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Ollama-specific implementation for AI-powered recipe analysis
/// </summary>
public class OllamaRecipeAIAnalysisService : IRecipeAIAnalysisService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;
    private const string ModelId = "gpt-oss:20b";

    public OllamaRecipeAIAnalysisService(Kernel kernel)
    {
        _kernel = kernel;
        _chatCompletionService = _kernel.Services.GetRequiredService<IChatCompletionService>();
    }

    /// <summary>
    /// Analyzes extracted text from an image to generate structured recipe data using Ollama
    /// </summary>
    /// <param name="extractedText">Text extracted from recipe image using OCR</param>
    /// <returns>JSON string containing structured recipe data with English and Vietnamese content</returns>
    /// <exception cref="ArgumentException">Thrown when extractedText is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when AI analysis fails</exception>
    public async Task<string> AnalyzeRecipeFromTextAsync(string extractedText)
    {
        if (string.IsNullOrWhiteSpace(extractedText))
            throw new ArgumentException("Extracted text cannot be null or empty", nameof(extractedText));

        try
        {
            var history = new ChatHistory();
            
            // Configure system message for recipe analysis using extracted constant
            history.AddSystemMessage(AIPrompts.RecipeAnalysisSystemMessage);
            history.AddUserMessage($"This is the text: {extractedText}");

            // Create Ollama-specific execution settings
            var executionSettings = new Microsoft.SemanticKernel.Connectors.Ollama.OllamaPromptExecutionSettings
            {
                ModelId = ModelId
            };

            var result = await _chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings,
                kernel: _kernel);

            return result.Content ?? throw new InvalidOperationException("AI analysis returned empty content");
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            throw new InvalidOperationException($"Failed to analyze recipe text using Ollama: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the name of the AI provider being used
    /// </summary>
    /// <returns>Provider name</returns>
    public string GetProviderName()
    {
        return "Ollama";
    }
}