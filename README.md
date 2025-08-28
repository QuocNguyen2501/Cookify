# # Cookify - Multi-Language Recipe Management System

A comprehensive recipe management application built with .NET 9.0 Aspire, featuring a Blazor administrative portal and a cross-platform mobile app using .NET MAUI.

## 🎯 Features

### 🌐 Multi-Language Support
- **English** and **Vietnamese** recipe content
- Seamless language switching in mobile app
- Localized database storage with JSON serialization

### 🖥️ Administrative Portal (Blazor WebAssembly)
- **Category Management**: Create, edit, and organize recipe categories
- **Recipe Management**: Full CRUD operations for recipes with multi-language support
- **Content Export**: Export recipes to JSON format for mobile app consumption
- **Responsive Design**: Tailwind CSS styling with modern UI

### 📱 Mobile App (.NET MAUI)
- **Cross-Platform**: Runs on Android, iOS, macOS, and Windows
- **Offline-First**: Uses locally stored JSON data for performance
- **Recipe Discovery**: Browse recipes by category with search functionality
- **Detailed Views**: Ingredient lists, step-by-step instructions with images
- **Language Toggle**: Switch between English and Vietnamese instantly
- **Ad Integration**: Banner ads on main page and recipe detail pages

### 🚀 Backend API (ASP.NET Core)
- **RESTful API**: Full CRUD operations for categories and recipes
- **Database**: SQLite with Entity Framework Core
- **Localization**: JSON-based storage for multi-language content
- **Export Endpoint**: Download recipes as JSON for mobile app sync

## 🏗️ Architecture

```
├── RecipeApp.AppHost/           # .NET Aspire Orchestrator
├── RecipeApp.ServiceDefaults/   # Shared Service Configuration
├── RecipeApp.ApiService/        # Backend API
│   ├── Controllers/             # API Controllers
│   ├── Data/                    # Entity Framework Context
│   └── Models/                  # Data Models
├── RecipePortal.WebApp/         # Blazor Administrative Portal
│   └── Components/Pages/        # Category/Recipe Management Pages
└── RecipeApp.Mobile/            # .NET MAUI Mobile App
    ├── ViewModels/              # MVVM ViewModels
    ├── Services/                # Data & Localization Services
    ├── Converters/              # Value Converters
    └── Resources/Raw/           # JSON Data & Images
```

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK** or later
- **Visual Studio 2022** (17.8+) or **VS Code** with C# extension
- **.NET MAUI workloads** installed

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/QuocNguyen2501/Cookify.git
   cd Cookify
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Install MAUI workloads** (if not already installed)
   ```bash
   dotnet workload install maui
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

### Running the Application

#### Option 1: Run with .NET Aspire (Recommended)
```bash
dotnet run --project RecipeApp.AppHost
```
This starts both the API service and Blazor portal with the Aspire dashboard.

#### Option 2: Run services individually
```bash
# Terminal 1 - API Service
dotnet run --project RecipeApp.ApiService

# Terminal 2 - Blazor Portal
dotnet run --project RecipePortal.WebApp

# Terminal 3 - Mobile App (Windows)
dotnet run --project RecipeApp.Mobile --framework net9.0-windows10.0.19041.0
```

## 📖 Usage Guide

### 1. **Content Management (Blazor Portal)**
- Navigate to the Blazor portal (typically `https://localhost:5001`)
- **Categories**: Create and manage recipe categories
  - Add category names in both English and Vietnamese
  - Upload category images
- **Recipes**: Create detailed recipes
  - Add recipe information in multiple languages
  - Specify ingredients, instructions, cooking time, difficulty
  - Upload recipe images
- **Export**: Download recipes as JSON for mobile app

### 2. **Mobile App Usage**
- **Browse Recipes**: View all recipes organized by category
- **Search**: Use the search toggle to find specific recipes
- **Language**: Switch between English and Vietnamese
- **Recipe Details**: Tap on any recipe to view:
  - Full description and cooking information
  - Ingredient list with measurements
  - Step-by-step cooking instructions
  - Recipe images

### 3. **Data Synchronization**
1. Create content in the Blazor portal
2. Export recipes using the "Export Recipes" button
3. Save the `recipes.json` file to `RecipeApp.Mobile/Resources/Raw/`
4. Add recipe images to `RecipeApp.Mobile/Resources/Raw/Images/`
5. Rebuild the mobile app to include new content

## 🗄️ Database Schema

### Categories Table
- `Id` (Primary Key)
- `NameData` (JSON) - Localized names
- `ImageFileName` - Image file reference

### Recipes Table
- `Id` (Primary Key)
- `CategoryId` (Foreign Key)
- `NameData` (JSON) - Localized names
- `DescriptionData` (JSON) - Localized descriptions
- `IngredientsData` (JSON) - Localized ingredient lists
- `InstructionsData` (JSON) - Localized instruction steps
- `CookingTimeMinutes` - Cooking duration
- `DifficultyLevel` - Easy/Medium/Hard
- `ImageFileName` - Image file reference

## 🔧 Configuration

### API Configuration
- **Database**: SQLite (default) - Can be changed in `appsettings.json`
- **CORS**: Configured for Blazor portal access
- **Swagger**: Available at `/swagger` endpoint

### Mobile App Configuration
- **Data Source**: Local JSON files in `Resources/Raw/`
- **Images**: Stored in `Resources/Raw/Images/`
- **Language**: Default to system language, switchable in app

## 🎨 Customization

### Adding New Languages
1. Update `RecipeLocalizedText.cs` with new language codes
2. Modify `LanguageService.cs` to support additional languages
3. Update UI language selectors
4. Add language-specific content in portal

### Styling
- **Blazor Portal**: Uses Tailwind CSS classes
- **Mobile App**: MAUI styles with theme binding for light/dark modes

## 📱 Platform-Specific Features

### Android
- Material Design components
- Native navigation patterns
- Banner and interstitial ad support

### iOS
- Native iOS design elements
- iOS-specific navigation
- AdMob integration ready

### Windows
- WinUI 3 controls
- Desktop-optimized layouts
- File system access for images

## 🔮 Future Enhancements

- **Real-time Sync**: WebSocket-based live updates
- **User Accounts**: Authentication and personal recipe collections
- **Recipe Ratings**: Community feedback system
- **Shopping Lists**: Generate ingredient shopping lists
- **Meal Planning**: Weekly meal planning features
- **Social Sharing**: Share favorite recipes
- **Recipe Import**: Import from web URLs or other formats

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📧 Support

For questions or support, please open an issue in the GitHub repository.

---

Built with ❤️ using .NET 9.0, Aspire, Blazor, and MAUI