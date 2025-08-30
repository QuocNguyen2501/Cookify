using System.ComponentModel;
using RecipeApp.Mobile.Services;

namespace RecipeApp.Mobile.Tests;

/// <summary>
/// Simple test class to validate language functionality
/// </summary>
public class LanguageTestRunner
{
    public static void TestLanguagePreferenceService()
    {
        Console.WriteLine("=== Testing Language Preference Service ===");
        
        var service = new LanguagePreferenceService();
        
        // Test initial state
        Console.WriteLine($"Initial language: {service.GetLanguagePreference()}");
        Console.WriteLine($"Has preference initially: {service.HasLanguagePreference()}");
        
        // Test setting language
        service.SetLanguagePreference("vi");
        Console.WriteLine($"After setting 'vi': {service.GetLanguagePreference()}");
        Console.WriteLine($"Has preference after setting: {service.HasLanguagePreference()}");
        
        // Test setting back to English
        service.SetLanguagePreference("en");
        Console.WriteLine($"After setting 'en': {service.GetLanguagePreference()}");
        
        Console.WriteLine("Language Preference Service test completed!");
    }
    
    public static void TestLanguageService()
    {
        Console.WriteLine("\n=== Testing Language Service ===");
        
        var preferenceService = new LanguagePreferenceService();
        var languageService = new LanguageService(preferenceService);
        
        // Subscribe to language changed event
        languageService.LanguageChanged += (newLanguage) =>
        {
            Console.WriteLine($"Language changed event: {newLanguage}");
        };
        
        Console.WriteLine($"Initial current language: {languageService.CurrentLanguage}");
        Console.WriteLine($"Is first time user: {languageService.IsFirstTimeUser()}");
        
        // Test setting language
        languageService.SetLanguage("vi");
        Console.WriteLine($"After setting 'vi': {languageService.CurrentLanguage}");
        
        languageService.SetLanguage("en");
        Console.WriteLine($"After setting 'en': {languageService.CurrentLanguage}");
        
        Console.WriteLine("Language Service test completed!");
    }
    
    public static void TestLanguageSelectionViewModel()
    {
        Console.WriteLine("\n=== Testing Language Selection ViewModel ===");
        
        var preferenceService = new LanguagePreferenceService();
        var languageService = new LanguageService(preferenceService);
        var viewModel = new ViewModels.LanguageSelectionPopupViewModel(preferenceService, languageService);
        
        Console.WriteLine($"ViewModel Title: {viewModel.Title}");
        Console.WriteLine($"Initial Selected Language: {viewModel.SelectedLanguage}");
        Console.WriteLine($"Available Language Options: {viewModel.LanguageOptions.Count}");
        
        foreach (var option in viewModel.LanguageOptions)
        {
            Console.WriteLine($"  - {option.DisplayText} (Code: {option.Code}, Selected: {option.IsSelected})");
        }
        
        // Test selecting Vietnamese
        viewModel.SelectLanguageCommand.Execute("vi");
        Console.WriteLine($"After selecting 'vi': {viewModel.SelectedLanguage}");
        
        foreach (var option in viewModel.LanguageOptions)
        {
            Console.WriteLine($"  - {option.DisplayText} (Selected: {option.IsSelected})");
        }
        
        Console.WriteLine("Language Selection ViewModel test completed!");
    }
    
    public static void RunAllTests()
    {
        Console.WriteLine("Starting Language Component Tests...\n");
        
        try
        {
            TestLanguagePreferenceService();
            TestLanguageService();
            TestLanguageSelectionViewModel();
            Console.WriteLine("\n=== All Tests Completed Successfully! ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n=== Test Failed: {ex.Message} ===");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
