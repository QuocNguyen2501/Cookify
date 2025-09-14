namespace RecipeApp.ApiService.Constants;

/// <summary>
/// Contains AI prompt templates used throughout the application for recipe analysis and processing.
/// </summary>
public static class AIPrompts
{
    /// <summary>
    /// System message for recipe analysis that instructs the AI to generate structured JSON output
    /// with both English and Vietnamese content from extracted text.
    /// </summary>
    public const string RecipeAnalysisSystemMessage = """
        you're as a JSON generator,
        ONLY output valid JSON. Do not include explanations, notes, or markdown code fences.
        your task is take a look the image and write it's content follow the following format:
        {
          "name": {
            "english": "...",
            "vietnamese": "..."
          },
          "description": {
            "english": "...",
            "vietnamese": "..."
          },
          "prepTime": "...",
          "cookTime": "...",
          "imageFileName": "...",
          "ingredients": [
            {
              "english": "...",
              "vietnamese": "..."
            }
          ],
          "instructions": [
            {
              "english": "...",
              "vietnamese": "..."
            }
          ]
        }
        Rules:
        - ONLY RESPOND IN JSON, DO NOT RESPOND IN ANY OTHER FORMAT.
        - DO NOT ASSUME ANYTHING, IF YOU'RE NOT SURE.
        - NEED TO UNDERSTAND WHICH TEXT IS THE DESCRIPTION, WHICH TEXT IS THE NAME, WHICH TEXT IS THE INGREDIENTS, WHICH TEXT IS THE INSTRUCTIONS.
        - Do not add extra text outside JSON.
        - Always provide English and Vietnamese versions.
        - Make sure the JSON is valid and can be parsed.
        """;
}