

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using RecipeApp.ApiService.Constants;

namespace RecipeApp.ApiService.Services;

/// <summary>
/// Azure OpenAI-specific implementation for AI-powered recipe analysis with vision capabilities
/// </summary>
public class AzureOpenAIRecipeAIAnalysisService : IRecipeAIAnalysisService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatCompletionService;

    public AzureOpenAIRecipeAIAnalysisService(Kernel kernel)
    {
        _kernel = kernel;
        _chatCompletionService = _kernel.Services.GetRequiredService<IChatCompletionService>();
    }

    /// <summary>
    /// Analyzes a base64-encoded recipe image using Azure OpenAI vision capabilities
    /// </summary>
    /// <param name="base64Image">Base64-encoded image string (without data URL prefix)</param>
    /// <returns>JSON string containing structured recipe data with English and Vietnamese content</returns>
    /// <exception cref="ArgumentException">Thrown when base64Image is null or empty</exception>
    /// <exception cref="InvalidOperationException">Thrown when AI analysis fails</exception>
    public async Task<string> AnalyzeRecipeFromTextAsync(string base64Image)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
            throw new ArgumentException("Base64 image cannot be null or empty", nameof(base64Image));

        try
        {
            var history = new ChatHistory();

            // Configure system message for recipe analysis using extracted constant
            history.AddSystemMessage(AIPrompts.RecipeAnalysisSystemMessage);

            // Create multimodal user message with both text and image content
            var userMessageContent = new ChatMessageContentItemCollection
            {
                // Add text instruction
                new TextContent("Please analyze this recipe image and extract the recipe information in the specified JSON format:"),
                // Add image content with base64 data URL
                new ImageContent(
                dataUri: $"{base64Image}")
            };

            // Create and add the multimodal user message
            var userMessage = new ChatMessageContent(
                role: AuthorRole.User,
                content: null, // No simple text content, using items instead
                modelId: null,
                innerContent: null,
                encoding: null,
                metadata: null)
            {
                Items = userMessageContent
            };

            history.Add(userMessage);

            // Create execution settings with max tokens for vision requests
            var executionSettings = new PromptExecutionSettings
            {
                ExtensionData = new Dictionary<string, object>
                {
                    ["max_tokens"] = 2000 // Important: Required for vision requests to avoid truncated output
                }
            };

            var result = await _chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings,
                kernel: _kernel);

            return result.Content ?? throw new InvalidOperationException("AI analysis returned empty content");
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            throw new InvalidOperationException($"Failed to analyze recipe image using Azure OpenAI: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets the name of the AI provider being used
    /// </summary>
    /// <returns>Provider name</returns>
    public string GetProviderName()
    {
        return "Azure OpenAI";
    }
}
