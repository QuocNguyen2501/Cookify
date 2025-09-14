using Microsoft.SemanticKernel;

namespace RecipeApp.ApiService.Extensions;

/// <summary>
/// Extension methods for configuring Ollama services in the dependency injection container.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds Ollama chat completion service with default configuration for recipe analysis.
    /// </summary>
    /// <param name="services">The service collection to add the service to.</param>
    /// <param name="modelName">The name of the Ollama model to use (default: "gpt-oss:20b").</param>
    /// <param name="baseAddress">The base address of the Ollama service (default: "http://localhost:56772/").</param>
    /// <param name="timeoutMinutes">The timeout in minutes for HTTP requests (default: 60).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOllamaForRecipeAnalysis(
        this IServiceCollection services,
        string modelName = "gpt-oss:20b",
        string baseAddress = "http://localhost:56772/",
        int timeoutMinutes = 60)
    {
        // Create custom HttpClient for Ollama with explicit timeout configuration
        var ollamaHttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(timeoutMinutes),
            BaseAddress = new Uri(baseAddress)
        };

        // Add Ollama chat completion with custom HttpClient
        services.AddOllamaChatCompletion(modelName, ollamaHttpClient);

        return services;
    }

    public static IServiceCollection AddAzureOpenAIForRecipeAnalysis(
        this IServiceCollection services,
        string modelId,
        string endpoint,
        string apiKey
    ){
        services.AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);
        return services;
    }
}