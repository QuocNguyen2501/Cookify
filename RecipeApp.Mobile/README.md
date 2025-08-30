# 📱 RecipeApp.Mobile - Cookify Mobile Application

A cross-platform mobile application built with .NET MAUI 9.0 for browsing and viewing Vietnamese recipes with bilingual support (English/Vietnamese).

## 🏗️ Architecture Overview

This mobile app is part of the **Cookify Recipe Management System** ecosystem, working alongside:
- **RecipeApp.ApiService** - Backend API with SQL Server database
- **RecipePortal.WebApp** - Blazor administrative portal for content management

### Key Design Principles
- **Offline-First**: Recipes stored locally in JSON format for offline access
- **Bilingual Support**: Full English/Vietnamese localization using `RecipeLocalizedText` model
- **MVVM Pattern**: Clean separation of concerns with ViewModels and data binding
- **Cross-Platform**: Single codebase targeting Android (extendable to iOS, Windows, macOS)

## 🚀 Features

### ✨ Core Functionality
- **Recipe Browsing**: View all recipes with categories, prep/cook times, and images
- **Recipe Details**: Full recipe view with ingredients, instructions, and images
- **Language Switching**: Toggle between English and Vietnamese instantly
- **Offline Access**: No internet connection required for recipe viewing
- **Category Filtering**: Browse recipes by category (Appetizers, Main Courses, Desserts, etc.)

### 🎨 User Experience
- **Native Performance**: Built with .NET MAUI for platform-specific optimizations
- **Responsive Design**: Adapts to different screen sizes and orientations
- **Intuitive Navigation**: Simple, clean interface following platform conventions
- **Image Support**: Recipe and category images with fallback handling

## 🛠️ Technical Stack

### Framework & Platform
- **.NET 9.0** - Latest .NET framework
- **.NET MAUI 9.0** - Multi-platform App UI framework
- **Target Platforms**: Android 5.0+ (API Level 21+)
- **C# 12** - Latest C# language features

### Architecture Patterns
- **MVVM (Model-View-ViewModel)** - Clean separation of UI and business logic
- **Dependency Injection** - Built-in DI container for service management
- **Singleton Services** - Shared data and language services
- **Data Binding** - Two-way binding between Views and ViewModels

### Key Libraries
- **Microsoft.Maui.Controls** 9.0.100 - Core MAUI controls
- **System.Text.Json** - JSON serialization for recipe data
- **INotifyPropertyChanged** - Property change notifications for data binding

## 📁 Project Structure

```
RecipeApp.Mobile/
├── Models/                     # Data models
│   ├── Recipe.cs              # Recipe entity with localized content
│   ├── Category.cs            # Category entity with localized names
│   └── RecipeLocalizedText.cs # Bilingual text container
├── ViewModels/                # MVVM ViewModels
│   ├── BaseViewModel.cs       # Base ViewModel with INotifyPropertyChanged
│   ├── MainViewModel.cs       # Recipe list logic
│   └── RecipeDetailViewModel.cs # Recipe detail logic
├── Services/                  # Business logic services
│   ├── RecipeDataService.cs   # JSON data loading and caching
│   └── LanguageService.cs     # Language switching functionality
├── Views/                     # XAML user interface
│   ├── MainPage.xaml         # Recipe list page
│   └── RecipeDetailPage.xaml # Recipe detail page
├── Resources/                 # App resources
│   ├── Raw/recipes.json      # Embedded recipe data
│   ├── Images/               # Recipe and UI images
│   ├── Fonts/                # Custom fonts
│   └── AppIcon/              # Application icons
└── Platforms/                # Platform-specific code
    └── Android/              # Android-specific implementations
```

## 🔧 Getting Started

### Prerequisites
- **Visual Studio 2022** (17.8+) or **Visual Studio Code**
- **.NET 9.0 SDK** or later
- **Android SDK** (for Android development)
- **MAUI Workload** installed

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/QuocNguyen2501/Cookify.git
   cd Cookify/RecipeApp.Mobile
   ```

2. **Install MAUI workload** (if not already installed)
   ```bash
   dotnet workload install maui
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

### Running the Application

#### Android Emulator/Device
```bash
dotnet build -t:Run -f net9.0-android
```

#### Visual Studio
1. Set `RecipeApp.Mobile` as startup project
2. Select Android emulator or connected device
3. Press F5 or click "Start Debugging"

## 🧭 Navigation Flow

The mobile app follows a **category-first navigation pattern** that prioritizes content discovery and intuitive user experience:

```
📱 MainPage (Category Grid)
    ↓ Tap Category Card
📋 CategoryRecipesPage (Recipe List)
    ↓ Tap Recipe Item
🍽️ RecipeDetailPage (Recipe Details)
    ↓ Back Navigation
📋 CategoryRecipesPage
    ↓ Back Navigation  
📱 MainPage
```

### 🏠 MainPage - Category Grid View
**Purpose**: Primary landing page showcasing recipe categories in an intuitive grid layout

**Features**:
- **2-Column Grid**: Optimized layout for mobile viewing with category cards
- **Visual Categories**: Each category displays with image and overlay text
- **Bilingual Support**: Category names shown in current language (English/Vietnamese)
- **Tap Navigation**: Direct navigation to category recipes via tap gestures
- **Language Toggle**: Switch between English/Vietnamese instantly

**User Journey**:
1. User opens app and sees category grid
2. Categories load from `categories.json` with localized names
3. User taps a category card (e.g., "Main Dishes" / "Món chính")
4. Navigation to CategoryRecipesPage with selected category

**ViewModel**: `MainViewModel`
- **Categories**: ObservableCollection of category cards
- **Commands**: `GoToCategoryCommand` for navigation
- **Properties**: Current language, loading states

### � CategoryRecipesPage - Filtered Recipe List
**Purpose**: Display all recipes within the selected category with search and filtering capabilities

**Features**:
- **Category Header**: Shows selected category name in current language
- **Recipe List**: All recipes filtered by the selected category
- **Search Functionality**: Toggle search bar to filter recipes by name
- **Recipe Cards**: Display recipe name, image, prep/cook time per card
- **Back Navigation**: Return to category grid view
- **Empty State**: Friendly message when no recipes match search

**Navigation Parameters**:
- **CategoryId**: GUID passed via query string from MainPage
- **Route**: `categoryrecipes?categoryId={guid}`

**User Journey**:
1. User arrives from MainPage with selected category
2. Page loads all recipes for that category
3. User can search/filter recipes by name (optional)
4. User taps recipe to view full details
5. User can navigate back to category grid

**ViewModel**: `CategoryRecipesViewModel`
- **QueryProperty**: `[QueryProperty("CategoryId", "categoryId")]`
- **CurrentCategory**: Selected category object with localized name
- **FilteredRecipes**: Observable collection filtered by category and search
- **Commands**: `GoToRecipeDetailCommand`, `GoBackCommand`, `ToggleSearchCommand`

### 🍽️ RecipeDetailPage - Full Recipe View  
**Purpose**: Complete recipe information with ingredients, instructions, and images

**Features**:
- **Recipe Header**: Name, category, prep/cook times
- **Ingredient List**: Localized ingredients with measurements
- **Step Instructions**: Numbered cooking steps in current language
- **Recipe Image**: High-quality recipe photo with fallback
- **Back Navigation**: Return to category recipe list
- **Language Switching**: All content updates instantly when language changes

**Navigation Parameters**:
- **RecipeId**: GUID passed via query string from CategoryRecipesPage
- **Route**: `recipedetail?recipeId={guid}`

**User Journey**:
1. User arrives from CategoryRecipesPage with selected recipe
2. Full recipe loads with all details in current language
3. User can scroll through ingredients and instructions
4. User navigates back to category recipe list

**ViewModel**: `RecipeDetailViewModel`
- **QueryProperty**: `[QueryProperty("RecipeId", "recipeId")]`
- **CurrentRecipe**: Full recipe object with localized content
- **Properties**: Localized ingredients, instructions, category info
- **Commands**: Back navigation command

### 🔄 Navigation Architecture

**Shell-Based Navigation**:
```csharp
// AppShell.xaml.cs - Route Registration
Routing.RegisterRoute("categoryrecipes", typeof(CategoryRecipesPage));
Routing.RegisterRoute("recipedetail", typeof(RecipeDetailPage));
```

**Command-Based Navigation**:
```csharp
// Modern MVVM with CommunityToolkit.Mvvm
[RelayCommand]
private async Task GoToCategory(Category category)
{
    await Shell.Current.GoToAsync($"categoryrecipes?categoryId={category.Id}");
}

[RelayCommand]
private async Task GoToRecipeDetail(Recipe recipe)
{
    await Shell.Current.GoToAsync($"recipedetail?recipeId={recipe.Id}");
}
```

**Query Parameters**:
```csharp
// Automatic parameter binding with source generation
[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class CategoryRecipesViewModel : BaseViewModel
{
    [ObservableProperty]
    private string categoryId = string.Empty;
    
    partial void OnCategoryIdChanged(string value)
    {
        LoadCategoryAsync(value);
    }
}
```

### 📱 User Experience Design

**Intuitive Flow**:
1. **Discovery**: Categories presented visually for easy browsing
2. **Filtering**: Natural category-based organization 
3. **Search**: Quick filtering within categories when needed
4. **Details**: Complete recipe information with easy navigation back

**Visual Hierarchy**:
- **Primary**: Category cards with images and clear labels
- **Secondary**: Recipe lists with essential info (name, time, image)
- **Detailed**: Full recipe view with comprehensive information

**Performance Optimizations**:
- **Lazy Loading**: Categories and recipes load on-demand
- **Efficient Navigation**: Direct object passing via query parameters
- **Memory Management**: ViewModels cleanup when navigating away
- **Smooth Transitions**: Native platform navigation animations

### 🔧 Technical Implementation

**Dependency Injection**:
```csharp
// MauiProgram.cs - Service Registration
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<CategoryRecipesViewModel>();
builder.Services.AddTransient<RecipeDetailViewModel>();

builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<CategoryRecipesPage>();
builder.Services.AddTransient<RecipeDetailPage>();
```

**Modern MVVM Patterns**:
- **Source Generation**: `[ObservableProperty]` and `[RelayCommand]` attributes
- **Query Properties**: Automatic parameter binding from navigation
- **Command Binding**: Declarative command definitions in XAML
- **Data Binding**: Two-way binding between Views and ViewModels

## �📊 Data Flow

### Recipe Data Management
1. **Source**: Recipes exported from RecipePortal.WebApp as JSON
2. **Storage**: Embedded in mobile app as `Resources/Raw/recipes.json`
3. **Loading**: `RecipeDataService` loads and caches data on app startup
4. **Access**: ViewModels retrieve data through service layer

### Category Data Management
1. **Source**: Categories exported from RecipePortal.WebApp as JSON
2. **Storage**: Embedded in mobile app as `Resources/Raw/categories.json`
3. **Images**: Category images stored in `Resources/Images/` folder
4. **Filtering**: Recipes filtered by category ID for navigation

### Language Management
1. **Service**: `LanguageService` manages current language state
2. **Storage**: Language preference persisted locally
3. **Switching**: Instant language changes without app restart
4. **Binding**: UI automatically updates through data binding

## 🔨 Key Components

### Models

#### RecipeLocalizedText
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

#### Recipe Model
```csharp
public class Recipe
{
    public Guid Id { get; set; }
    public RecipeLocalizedText Name { get; set; } = new();
    public RecipeLocalizedText Description { get; set; } = new();
    public Category? Category { get; set; }
    public string PrepTime { get; set; } = string.Empty;
    public string CookTime { get; set; } = string.Empty;
    public List<RecipeLocalizedText> Ingredients { get; set; } = new();
    public List<RecipeLocalizedText> Instructions { get; set; } = new();
    public string? ImageFileName { get; set; }
}
```

### Services

#### RecipeDataService
- **Purpose**: Loads and caches recipe data from embedded JSON
- **Pattern**: Singleton with lazy loading
- **Methods**: `GetRecipesAsync()`, `GetCategoriesAsync()`, `GetRecipeByIdAsync()`

#### LanguageService
- **Purpose**: Manages application language state
- **Pattern**: Singleton with property change notifications
- **Events**: `LanguageChanged` event for UI updates

### ViewModels

#### MainViewModel
- **Purpose**: Manages recipe list and filtering logic
- **Features**: Category filtering, search, language switching
- **Commands**: Navigation to recipe details

#### RecipeDetailViewModel
- **Purpose**: Displays detailed recipe information
- **Features**: Localized content display, image handling
- **Navigation**: Receives recipe ID via query parameters

## 🌐 Localization

### Language Support
- **English (en)** - Primary language
- **Vietnamese (vi)** - Secondary language with fallback to English

### Implementation
- **Model Level**: `RecipeLocalizedText` handles bilingual content
- **Service Level**: `LanguageService` manages current language
- **UI Level**: Data binding automatically updates text based on language

### Adding New Languages
1. Extend `RecipeLocalizedText.GetLocalizedText()` method
2. Add language option to UI language selector
3. Update JSON data export to include new language content

## 📱 Platform Support

### Current Support
- **Android 5.0+** (API Level 21+)
- **Minimum SDK**: Android 5.0 (API 21)
- **Target SDK**: Latest Android API

### Future Platform Extensions
The app is designed for easy extension to:
- **iOS** - iPhone and iPad support
- **Windows** - Windows 10/11 desktop app
- **macOS** - Mac desktop application

### Platform-Specific Features
- **Android**: Native navigation, Material Design elements
- **Cross-Platform**: Shared business logic and UI components

## 🔄 Integration with Backend

### Data Synchronization
1. **Export**: Use RecipePortal.WebApp to export recipes as JSON
2. **Update**: Replace `Resources/Raw/recipes.json` with new data
3. **Rebuild**: Recompile mobile app with updated content
4. **Deploy**: Distribute updated app to users

### JSON Schema
```json
[
  {
    "id": "guid",
    "name": {
      "english": "Recipe Name",
      "vietnamese": "Tên Công Thức"
    },
    "description": {
      "english": "Description",
      "vietnamese": "Mô Tả"
    },
    "category": {
      "id": "guid",
      "name": {
        "english": "Category",
        "vietnamese": "Danh Mục"
      }
    },
    "ingredients": [
      {
        "english": "Ingredient",
        "vietnamese": "Nguyên Liệu"
      }
    ],
    "instructions": [
      {
        "english": "Step",
        "vietnamese": "Bước"
      }
    ]
  }
]
```

## 🚀 Deployment

### Android Deployment
1. **Debug Build**: For testing on emulators/devices
   ```bash
   dotnet build -c Debug -f net9.0-android
   ```

2. **Release Build**: For production deployment
   ```bash
   dotnet build -c Release -f net9.0-android
   ```

3. **APK Generation**:
   ```bash
   dotnet publish -c Release -f net9.0-android
   ```

### Play Store Deployment
1. **Signing**: Configure signing certificate
2. **Bundle**: Generate AAB (Android App Bundle)
3. **Upload**: Submit to Google Play Console
4. **Review**: Google Play review process

## 🧪 Testing

### Unit Testing
- **ViewModels**: Test business logic and data binding
- **Services**: Test data loading and language management
- **Models**: Test localization logic

### Integration Testing
- **Data Loading**: Test JSON parsing and caching
- **Navigation**: Test page navigation and parameter passing
- **UI Binding**: Test data binding and property changes

### Device Testing
- **Multiple Devices**: Test on various Android devices and screen sizes
- **Performance**: Memory usage and app startup time
- **Offline**: Test functionality without internet connection

## 🤝 Contributing

### Development Workflow
1. **Fork** the repository
2. **Create** feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** changes (`git commit -m 'Add amazing feature'`)
4. **Push** to branch (`git push origin feature/amazing-feature`)
5. **Open** Pull Request

### Coding Standards
- **C# Conventions**: Follow Microsoft C# coding conventions
- **XAML Standards**: Use consistent naming and structure
- **Comments**: Document complex logic and public APIs
- **Testing**: Include unit tests for new features

## 📚 Resources

### Documentation
- [.NET MAUI Documentation](https://docs.microsoft.com/en-us/dotnet/maui/)
- [MVVM Pattern Guide](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm)
- [Android Development](https://developer.android.com/)

### Learning Resources
- [.NET MAUI Tutorial](https://docs.microsoft.com/en-us/dotnet/maui/get-started/first-app)
- [XAML Fundamentals](https://docs.microsoft.com/en-us/dotnet/maui/xaml/)
- [Data Binding in MAUI](https://docs.microsoft.com/en-us/dotnet/maui/fundamentals/data-binding/)

## 📄 License

This project is part of the **Cookify Recipe Management System**.

## 🆘 Support

### Getting Help
- **Issues**: Open GitHub issues for bugs and feature requests
- **Discussions**: Use GitHub Discussions for questions
- **Documentation**: Check this README and inline code comments

### Known Issues
- **Image Loading**: Some images may require internet connection for first load
- **Large Datasets**: Performance optimization needed for 1000+ recipes
- **Memory**: Monitor memory usage with large recipe collections

## 🔮 Roadmap

### Upcoming Features
- **Search Functionality**: Full-text search across recipes
- **Favorites**: Mark and filter favorite recipes
- **Shopping Lists**: Generate shopping lists from recipes
- **Recipe Sharing**: Share recipes via platform sharing
- **Offline Maps**: Integration with recipe locations

### Technical Improvements
- **Performance**: Virtualization for large recipe lists
- **Caching**: Enhanced image caching mechanisms
- **Testing**: Automated UI testing with Device Testing
- **CI/CD**: Automated build and deployment pipelines

---

**Built with ❤️ using .NET MAUI 9.0 and the power of cross-platform development**