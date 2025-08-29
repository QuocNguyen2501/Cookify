# Cookify - AI Agent Instructions

## 🎯 Project Overview

Cookify is a **multi-language recipe management system** with Vietnamese and English support, built using .NET 9.0 Aspire architecture. This is a **complete, production-ready solution** with three core components:

1. **RecipeApp.ApiService** - ASP.NET Core Web API with SQL Server database
2. **RecipePortal.WebApp** - Blazor WebAssembly administrative portal 
3. **RecipeApp.Mobile** - .NET MAUI cross-platform mobile application

**Critical Context**: This project is 100% complete and functional. Focus on understanding the existing architecture before making any changes.

## 🏗️ Architecture Principles

### .NET Aspire Orchestration
- **AppHost Project**: `RecipeApp.AppHost` orchestrates all services via `Program.cs`
- **SQL Server Container**: Managed by Aspire with automatic service discovery
- **Database Management**: `recipesdb` database created and managed through Aspire
- **Service References**: WebApp references ApiService for HTTP communication
- **Service Defaults**: Shared configuration in `RecipeApp.ServiceDefaults`
- **Run Command**: Always use `dotnet run --project RecipeApp.AppHost` for development

### Multi-Language Data Model
The **core innovation** is `RecipeLocalizedText` class for seamless bilingual support:

```csharp
public class RecipeLocalizedText
{
    public string English { get; set; } = string.Empty;
    public string Vietnamese { get; set; } = string.Empty;
    
    public string GetLocalizedText(string languageCode) => 
        languageCode?.ToLower() switch
        {
            "vi" => !string.IsNullOrWhiteSpace(Vietnamese) ? Vietnamese : English,
            "en" or _ => English
        };
}
```

**Key Usage Pattern**: Every user-facing text field (names, descriptions, ingredients, instructions) uses `RecipeLocalizedText` objects.

### Database Architecture (SQL Server + EF Core)
- **Container Orchestration**: SQL Server runs in Docker container managed by Aspire
- **Connection Management**: Automatic connection string injection via Aspire service discovery
- **Retry Logic**: `EnableRetryOnFailure` configured for transient error handling
- **JSON Serialization**: `RecipeLocalizedText` objects stored as JSON columns via EF Core value converters
- **Relationship**: `Recipe` → `Category` (Many-to-One with navigation properties)
- **Database Name**: `recipesdb` (managed by SQL Server container)
- **Seeding**: `DatabaseSeeder.cs` populates sample Vietnamese recipes on startup

## 🔧 Development Guidelines

### When Working with Models
- **NEVER** use simple strings for user content - always use `RecipeLocalizedText`
- **Recipe Properties**: Name, Description, Ingredients[], Instructions[] are all localized
- **Category Properties**: Name is localized
- **Data Consistency**: Ensure both English and Vietnamese fields are populated

### API Development Patterns
- **Controllers**: Follow existing patterns in `CategoriesController` and `RecipesController`
- **Export Endpoint**: `GET /api/recipes/export` returns JSON with embedded Category data for mobile app
- **Include Strategy**: Always `.Include(r => r.Category)` when fetching recipes
- **Return Types**: Use appropriate HTTP status codes (200, 201, 204, 404)

### Blazor Frontend Conventions
- **Tailwind CSS**: All styling uses Tailwind utility classes
- **Form Patterns**: Study `RecipeEdit.razor` for proper form validation and HttpClient usage
- **Page Structure**: Components in `Components/Pages/` with clear page titles
- **Error Handling**: User-friendly error messages and loading states

### MAUI Mobile App Architecture
- **MVVM Pattern**: ViewModels in `ViewModels/`, inherit from `BaseViewModel`
- **Dependency Injection**: Register services, ViewModels, and Pages in `MauiProgram.cs`
- **Data Layer**: `RecipeDataService` loads from embedded `recipes.json` file
- **Navigation**: Shell-based navigation via `AppShell.xaml`
- **Localization**: `LanguageService` manages current language state

## 📁 Key File Locations

### Essential Files to Understand
```
RecipeApp.ApiService/
├── Models/
│   ├── RecipeLocalizedText.cs     # Core localization model
│   ├── Recipe.cs                  # Main recipe entity
│   └── Category.cs                # Category entity
├── Data/
│   ├── AppDbContext.cs           # EF Core context with JSON conversions
│   └── DatabaseSeeder.cs         # Sample data seeding
└── Controllers/
    ├── RecipesController.cs      # CRUD + Export endpoint
    └── CategoriesController.cs   # Category management

RecipePortal.WebApp/Components/Pages/
├── RecipeList.razor              # Recipe management with export
├── RecipeEdit.razor              # Recipe creation/editing
├── CategoryList.razor            # Category management
└── CategoryEdit.razor            # Category creation/editing

RecipeApp.Mobile/
├── ViewModels/
│   ├── MainViewModel.cs          # Recipe list logic
│   └── RecipeDetailViewModel.cs  # Recipe details logic
├── Services/
│   ├── RecipeDataService.cs      # JSON data loading
│   └── LanguageService.cs        # Language management
└── Pages/
    ├── MainPage.xaml             # Recipe list UI
    └── RecipeDetailPage.xaml     # Recipe detail UI
```

### Configuration Files
- `RecipeApp.AppHost/Program.cs` - Aspire orchestration
- `RecipeApp.Mobile/MauiProgram.cs` - DI container setup
- `RecipePortal.WebApp/Program.cs` - Blazor configuration

## 🚨 Common Patterns & Anti-Patterns

### ✅ DO - Follow These Patterns
- Use `RecipeLocalizedText` for all user content
- Include navigation properties when querying (`Include(r => r.Category)`)
- Register services properly in each project's Program.cs/MauiProgram.cs
- Follow MVVM pattern strictly in MAUI app
- Use Tailwind CSS classes for styling in Blazor
- Handle loading states and errors gracefully

### ❌ DON'T - Avoid These Mistakes
- Don't use plain strings for user-facing content
- Don't forget to seed the database on first run
- Don't break the JSON schema for mobile app export
- Don't mix UI logic into ViewModels (use Commands)
- Don't hardcode language strings (use .resx for static text)

## 🔄 Typical Development Workflows

### Adding New Recipe Fields
1. Update `Recipe.cs` model with `RecipeLocalizedText` property
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
1. Update ViewModels first (data binding properties)
2. Modify XAML pages with proper data binding
3. Register new ViewModels/Pages in `MauiProgram.cs` if added
4. Test language switching functionality

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

---

**Remember**: This is a complete, working solution. Study the existing patterns thoroughly before making changes. The multi-language architecture using `RecipeLocalizedText` is the key innovation that makes this system work seamlessly across all platforms.
