# Cookify - AI Agent Instructions

## 🎯 Project Overview

Cookify is a **comprehensive multi-language recipe management system** with Vietnamese and English support, built using .NET 9.0 Aspire architecture with advanced AI capabilities. This is a **complete, production-ready solution** with six core components:

1. **RecipeApp.Models** - Shared class library containing all data models with comprehensive localization
2. **RecipeApp.ApiService** - ASP.NET Core Web API with SQL Server database, AI integration via Ollama, and OCR capabilities
3. **RecipePortal.WebApp** - Blazor Server administrative portal with Facet mapping
4. **RecipeApp.Mobile** - .NET MAUI cross-platform mobile application with CommunityToolkit.Mvvm and ad integration
5. **RecipeApp.AppHost** - .NET Aspire orchestration with SQL Server and Ollama services
6. **RecipeApp.ServiceDefaults** - Shared Aspire service configuration

**Critical Context**: This project is 100% complete and functional with advanced AI capabilities for recipe analysis from images. Focus on understanding the existing architecture before making any changes.

## 🔧 Key Technologies & Dependencies

### Core Framework
- **.NET 9.0**: Latest .NET version for all projects
- **.NET Aspire**: Cloud-native orchestration and service discovery
- **Entity Framework Core 9.0**: Data access with SQL Server provider
- **ASP.NET Core 9.0**: Web API and Blazor Server

### AI & Image Processing
- **Microsoft Semantic Kernel 1.65.0**: AI orchestration and chat completion
- **Ollama Connector**: Local AI model integration (gpt-oss:20b)
- **Tesseract 5.2.0**: OCR for Vietnamese text recognition
- **Request Timeouts**: 60-minute policies for AI operations

### Mobile Development
- **.NET MAUI**: Cross-platform mobile application framework
- **CommunityToolkit.Mvvm 8.4.0**: MVVM pattern implementation
- **CommunityToolkit.Maui 9.1.0**: Enhanced UI controls and behaviors
- **Plugin.MauiMTAdmob 2.0.2**: Mobile advertising integration

### Web Development
- **Blazor Server**: Interactive server-side rendering
- **Tailwind CSS**: Utility-first styling framework
- **Facet 2.4.5**: Model mapping for form binding

### Database & Storage
- **SQL Server**: Containerized database via Aspire
- **JSON Columns**: EF Core value converters for localized text
- **Data Volumes**: Persistent storage for SQL Server and Ollama

## 🏗️ Architecture Principles

### .NET Aspire Orchestration
- **AppHost Project**: `RecipeApp.AppHost` orchestrates all services via `Program.cs`
- **SQL Server Container**: Managed by Aspire with automatic service discovery and data volumes
- **Ollama Integration**: Local AI service running on port 56772 with gpt-oss:20b model for recipe analysis
- **Database Management**: `recipesdb` database created and managed through Aspire
- **Service References**: WebApp references ApiService for HTTP communication
- **Service Defaults**: Shared configuration in `RecipeApp.ServiceDefaults` with OpenTelemetry and health checks
- **Run Command**: Always use `dotnet run --project RecipeApp.AppHost` for development

### Enhanced Models Architecture
- **RecipeApp.Models**: Centralized class library containing all data models
- **Cross-Project Consistency**: Single source of truth for Recipe, Category, and RecipeLocalizedText
- **Enhanced Models**: Models include comprehensive validation attributes, constructors, and helper methods
- **Implicit Conversions**: RecipeLocalizedText supports implicit string conversions for ease of use
- **Project References**: All projects reference RecipeApp.Models for consistent data structures
- **No Model Duplication**: Models are maintained in one location and shared across all applications
- **Equality & Hashing**: Proper equality comparison and hash code generation implemented

### Multi-Language Data Model
The **core innovation** is `RecipeLocalizedText` class for seamless bilingual support:

```csharp
/// <summary>
/// Represents localized text content supporting both English and Vietnamese languages.
/// This is the core model for multi-language support across the Cookify application.
/// </summary>
public class RecipeLocalizedText
{
    [Required(ErrorMessage = "English text is required")]
    [StringLength(1000, ErrorMessage = "English text cannot exceed 1000 characters")]
    public string English { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Vietnamese text cannot exceed 1000 characters")]
    public string Vietnamese { get; set; } = string.Empty;

    /// <summary>
    /// Gets the localized text based on the provided language code.
    /// Defaults to English if the language code is unknown or if the specified language text is null/empty.
    /// </summary>
    /// <param name="languageCode">The language code (e.g., "en" for English, "vi" for Vietnamese)</param>
    /// <returns>The localized text in the specified language</returns>
    public string GetLocalizedText(string languageCode)
    {
        return languageCode?.ToLower() switch
        {
            "vi" => !string.IsNullOrWhiteSpace(Vietnamese) ? Vietnamese : English,
            "en" or _ => English
        };
    }

    // Constructors, implicit conversions, equality, and other utility methods included
}
```

**Key Usage Pattern**: Every user-facing text field (names, descriptions, ingredients, instructions) uses `RecipeLocalizedText` objects.

### Database Architecture (SQL Server + EF Core)
- **Container Orchestration**: SQL Server runs in Docker container managed by Aspire
- **Connection Management**: Automatic connection string injection via Aspire service discovery
- **Retry Logic**: `EnableRetryOnFailure` configured for transient error handling
- **JSON Serialization**: `RecipeLocalizedText` objects stored as JSON columns via EF Core value converters
- **Custom Value Comparer**: `ListRecipeLocalizedTextValueComparer` enables proper change tracking for List properties
- **Relationship**: `Recipe` → `Category` (Many-to-One with navigation properties)
- **Database Name**: `recipesdb` (managed by SQL Server container)
- **Seeding**: `DatabaseSeeder.cs` populates sample Vietnamese recipes on startup
- **Migration Handling**: Smart migration logic handles switching from EnsureCreated to migrations

### AI Integration Architecture
- **Ollama Service**: Local AI service integrated via Aspire on port 56772
- **Model**: Uses gpt-oss:20b model for recipe analysis
- **Semantic Kernel**: Microsoft Semantic Kernel integration for AI operations
- **OCR Capabilities**: Tesseract OCR with Vietnamese language support
- **Image Processing**: Upload and analyze recipe images with AI-powered content extraction
- **Timeout Handling**: 60-minute timeout policy for AI operations to handle complex processing
- **Service Pattern**: `IRecipeAIAnalysisService` with Ollama-specific implementation

## 🔧 Development Guidelines

### When Working with Models
- **Shared Library**: All models are in `RecipeApp.Models` - never duplicate models in individual projects
- **NEVER** use simple strings for user content - always use `RecipeLocalizedText`
- **Recipe Properties**: Name, Description, Ingredients[], Instructions[] are all localized
- **Category Properties**: Name is localized
- **Data Consistency**: Ensure both English and Vietnamese fields are populated
- **Form Binding**: Use helper properties (IngredientsTextEnglish/Vietnamese) for web forms
- **Validation**: Models include comprehensive validation attributes for all usage contexts

### API Development Patterns
- **Controllers**: Follow existing patterns in `CategoriesController`, `RecipesController`, and `ImageAIController`
- **Service Layer**: Use service layer pattern for business logic (Recipe, Category, Image Processing, AI Analysis services)
- **Export Endpoint**: `GET /api/recipes/export` returns JSON with embedded Category data for mobile app
- **AI Analysis Endpoint**: `POST /api/imageai/analyse` accepts images and returns structured recipe data
- **Include Strategy**: Always `.Include(r => r.Category)` when fetching recipes
- **Return Types**: Use appropriate HTTP status codes (200, 201, 204, 404)
- **Error Handling**: Wrap operations in try-catch with proper status codes and error messages
- **JSON Serialization**: Uses System.Text.Json with PropertyNameCaseInsensitive and ReferenceHandler.IgnoreCycles
- **Timeout Management**: Request timeout policies for long-running AI operations

### Blazor Frontend Conventions
- **Tailwind CSS**: All styling uses Tailwind utility classes
- **Rendermode**: Uses `@rendermode InteractiveServer` for form pages
- **Form Patterns**: Study `RecipeEdit.razor` for proper form validation and HttpClient usage
- **PortalRecipe Model**: Uses dedicated `PortalRecipe` model with helper properties for form binding (IngredientsTextEnglish/Vietnamese, InstructionsTextEnglish/Vietnamese)
- **Facet Library**: Leverages Facet for model mapping between Recipe and PortalRecipe
- **Page Structure**: Components in `Components/Pages/` with clear page titles
- **Error Handling**: User-friendly error messages and loading states with spinner UI

### MAUI Mobile App Architecture
- **MVVM Pattern**: ViewModels in `ViewModels/`, inherit from `BaseViewModel`, use CommunityToolkit.Mvvm
- **RelayCommand**: Use `[RelayCommand]` attributes for command methods (no manual ICommand implementation)
- **ObservableProperty**: Use `[ObservableProperty]` for bindable properties with automatic change notifications
- **Dependency Injection**: Register services, ViewModels, and Pages in `MauiProgram.cs`
- **Data Layer**: `RecipeDataService` loads from embedded `recipes.json` file using FileSystem.OpenAppPackageFileAsync
- **Navigation**: Shell-based navigation via `AppShell.xaml` with query parameters
- **Localization**: `LanguageService` manages current language state with event notifications
- **Ad Integration**: Uses MTAdmob plugin with interstitial ads on category navigation (every 3rd click)
- **Offline-First**: App works entirely from local JSON data, no direct API calls
- **Community Toolkit**: Leverages CommunityToolkit.Maui for enhanced UI controls and behaviors

## 📁 Key File Locations

### Essential Files to Understand
```
RecipeApp.Models/
├── Recipe.cs                     # Main recipe entity with form binding helpers
├── Category.cs                   # Category entity with localization
└── RecipeLocalizedText.cs        # Core localization model

RecipeApp.ApiService/
├── Data/
│   ├── AppDbContext.cs           # EF Core context with JSON conversions
│   └── DatabaseSeeder.cs         # Sample data seeding
├── Controllers/
│   ├── RecipesController.cs      # CRUD + Export endpoint
│   ├── CategoriesController.cs   # Category management
│   └── ImageAIController.cs      # AI image analysis endpoint
├── Services/
│   ├── CategoryService.cs        # Business logic for categories
│   ├── RecipeService.cs          # Business logic for recipes
│   ├── ImageProcessingService.cs # OCR and image processing
│   ├── IRecipeAIAnalysisService.cs    # AI analysis interface
│   └── OllamaRecipeAIAnalysisService.cs # Ollama AI implementation
└── tessdata/
    └── vie.traineddata           # Vietnamese OCR language data

RecipePortal.WebApp/
├── Components/Pages/
│   ├── RecipeList.razor          # Recipe management with export
│   ├── RecipeEdit.razor          # Recipe creation/editing
│   ├── CategoryList.razor        # Category management
│   └── CategoryEdit.razor        # Category creation/editing
├── Models/
│   └── PortalRecipe.cs           # Form-binding model with helper properties
└── Mappers/
    └── ...                       # Facet model mapping configurations

RecipeApp.Mobile/
├── ViewModels/
│   ├── BaseViewModel.cs          # Base class with CommunityToolkit.Mvvm
│   ├── MainViewModel.cs          # Recipe list logic with CommunityToolkit.Mvvm
│   ├── RecipeDetailViewModel.cs  # Recipe details logic
│   ├── CategoryRecipesViewModel.cs # Category-specific recipes
│   ├── AppShellViewModel.cs      # Shell navigation logic
│   └── LanguageSelectionPopupViewModel.cs # Language selection
├── Services/
│   ├── RecipeDataService.cs      # JSON data loading from embedded files
│   ├── CategoryDataService.cs    # Category data management
│   ├── LanguageService.cs        # Language management with events
│   ├── LanguagePreferenceService.cs # Language preference persistence
│   └── AdService.cs              # MTAdmob integration for interstitial ads
├── Pages/
│   ├── MainPage.xaml             # Recipe list UI
│   ├── RecipeDetailPage.xaml     # Recipe detail UI
│   └── CategoryRecipesPage.xaml  # Category-specific recipes UI
└── Components/
    └── Popups/
        └── LanguageSelectionPopup.xaml # Language selection popup
```

### Configuration Files
- `RecipeApp.AppHost/Program.cs` - Aspire orchestration with Ollama and SQL Server
- `RecipeApp.Mobile/MauiProgram.cs` - DI container setup with CommunityToolkit
- `RecipePortal.WebApp/Program.cs` - Blazor configuration with Facet mapping
- `RecipeApp.ServiceDefaults/Extensions.cs` - Shared service configuration

## 🚨 Common Patterns & Anti-Patterns

### ✅ DO - Follow These Patterns
- Use `RecipeLocalizedText` for all user content
- Include navigation properties when querying (`Include(r => r.Category)`)
- Register services properly in each project's Program.cs/MauiProgram.cs
- Follow MVVM pattern strictly in MAUI app with CommunityToolkit.Mvvm
- Use Tailwind CSS classes for styling in Blazor
- Handle loading states and errors gracefully
- Use `[ObservableProperty]` and `[RelayCommand]` attributes in ViewModels
- Implement proper timeout handling for AI operations
- Use service layer pattern for business logic separation

### ❌ DON'T - Avoid These Mistakes
- Don't use plain strings for user-facing content
- Don't forget to seed the database on first run
- Don't break the JSON schema for mobile app export
- Don't mix UI logic into ViewModels (use Commands)
- Don't hardcode language strings (use .resx for static text)
- Don't manually implement ICommand or INotifyPropertyChanged (use CommunityToolkit)
- Don't bypass timeout policies for AI operations
- Don't duplicate models across projects

## 🔄 Typical Development Workflows

### Adding New Recipe Fields
1. Update `Recipe.cs` model in `RecipeApp.Models` with `RecipeLocalizedText` property
2. Update `AppDbContext.cs` with JSON conversion for the new field
3. Create and apply EF Core migration for SQL Server
4. Update API controllers to handle new field
5. Update Blazor forms in `RecipeEdit.razor`
6. Update mobile ViewModels and UI to display new field

### Adding New API Endpoints
1. Follow pattern in existing controllers (async/await, proper HTTP status codes)
2. Include navigation properties where needed
3. Test with Blazor frontend integration
4. Update mobile app if endpoint affects mobile data flow

### Modifying Mobile UI
1. Update ViewModels first (data binding properties with `[ObservableProperty]`)
2. Modify XAML pages with proper data binding
3. Register new ViewModels/Pages in `MauiProgram.cs` if added
4. Test language switching functionality

### Adding AI Features
1. Create service interface in `RecipeApp.ApiService/Services/`
2. Implement service with proper error handling and timeout management
3. Register service in `Program.cs` with appropriate lifetime
4. Add controller endpoints with `[RequestTimeout]` attributes
5. Test with various input scenarios and timeout conditions

## 🧩 Integration Points

### API ↔ Blazor Communication
- **HttpClient**: Injected in Blazor pages for API calls
- **Base URL**: Automatically configured via Aspire service discovery
- **Serialization**: System.Text.Json with default options

### API ↔ Mobile Data Flow
- **Export Process**: Blazor portal exports recipes via `/api/recipes/export`
- **Mobile Import**: JSON file placed in `Resources/Raw/recipes.json`
- **Build Action**: Ensure JSON file has `MauiAsset` build action

### Cross-Project Dependencies
- Mobile app is independent (no direct API calls)
- WebApp references ApiService via Aspire
- All projects reference RecipeApp.Models for shared data structures
- All projects target .NET 9.0

## 🎨 UI/UX Standards

### Blazor Portal
- **Design System**: Tailwind CSS utility-first approach
- **Color Scheme**: Blue primary (`bg-blue-500`), Gray secondary
- **Layout**: Container-based responsive design
- **Forms**: Proper validation with error display

### MAUI Mobile App
- **Navigation**: Shell-based with TabBar
- **Styling**: Platform-appropriate controls
- **Responsiveness**: Grid and StackLayout for various screen sizes
- **Language UI**: Instant switching without restart

## 📊 Database Schema

### Core Tables (SQL Server)
```sql
-- Categories: Localized category names
Categories (Id UNIQUEIDENTIFIER, Name NVARCHAR(MAX))

-- Recipes: Full recipe data with localized content
Recipes (Id UNIQUEIDENTIFIER, Name NVARCHAR(MAX), Description NVARCHAR(MAX), 
         PrepTime NVARCHAR(MAX), CookTime NVARCHAR(MAX), ImageFileName NVARCHAR(MAX),
         CategoryId UNIQUEIDENTIFIER, Ingredients NVARCHAR(MAX), Instructions NVARCHAR(MAX))
```

### Sample Data
- SQL Server database includes Vietnamese recipe examples
- Categories: "Món chính/Main Dishes", "Tráng miệng/Desserts"
- Ready-to-use sample data for development and testing
- Database automatically created and seeded on first run

## 🚀 Quick Start for New Developers

### First-Time Setup
```bash
# 1. Restore packages
dotnet restore

# 2. Install MAUI workloads  
dotnet workload install maui

# 3. Build solution
dotnet build

# 4. Run with Aspire (starts API + Blazor)
dotnet run --project RecipeApp.AppHost
```

### Immediate Next Steps
1. Open Aspire dashboard to see running services
2. Access Blazor portal to understand content management
3. Export sample data and examine JSON structure
4. Run mobile app to see end-user experience
5. Study `RecipeLocalizedText` usage patterns before any modifications

### Development Iteration
1. Make changes to API/Blazor for content management
2. Export updated JSON from Blazor portal
3. Update mobile app's `recipes.json` asset
4. Test mobile app with new content

## 🔍 Testing & Debugging

### Recommended Testing Flow
- **API**: Test endpoints via Swagger UI or browser
- **Blazor**: Use browser dev tools for debugging
- **Mobile**: Use platform-specific debuggers (Android/iOS simulators)
- **Integration**: Test complete export → import → mobile display workflow

### Common Issues & Solutions
- **Build Errors**: Ensure all projects target .NET 9.0
- **Mobile Loading**: Check `recipes.json` exists in Resources/Raw with MauiAsset build action  
- **Localization**: Verify both English and Vietnamese content is provided
- **API Calls**: Confirm Aspire service discovery is working properly
- **SQL Server Connection**: Ensure SQL Server container is running and retry logic is configured
- **Shared Models**: Ensure all projects reference RecipeApp.Models correctly
- **AI Operations**: Check Ollama service is running and timeout policies are configured
- **OCR Processing**: Verify tessdata folder contains Vietnamese language files

---

**Remember**: This is a complete, working solution with advanced AI capabilities for recipe analysis from images. Study the existing patterns thoroughly before making changes. The multi-language architecture using `RecipeLocalizedText` is the key innovation that makes this system work seamlessly across all platforms.
