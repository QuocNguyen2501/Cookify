# **Recipe App Specification**

## **1\. Introduction**

This document provides a highly detailed specification for the "Recipe Book App." This application comprises three main components:

1. **Mobile App**: A .NET MAUI mobile application for end-users to view recipes, **with full multi-language support (Vietnamese and English) for both Recipes and Categories**, and Google AdMob integration for monetization.  
2. **Portal Backend**: An API backend built with ASP.NET Aspire for multi-language recipe and category data management, database interaction, and JSON export.  
3. **Portal Frontend**: A Blazor user interface for entering, editing, and exporting **multi-language** recipe and category data to the mobile app.

The primary goal is to create a robust local development environment where multi-language content (including categorization) can be effortlessly managed and then packaged for a data-independent (offline-first) mobile application, with a clear strategy for advertising revenue and internationalization.

## **2\. Technologies Used**

* **Mobile App**: .NET MAUI (targeting iOS, Android, Windows) with built-in localization features.  
* **Portal Backend**: ASP.NET Aspire **9.0** (for orchestration, service discovery, and telemetry), ASP.NET Core Web API.  
* **Portal Frontend**: Blazor WebAssembly **9.0** (hosted in Aspire, for interactive client-side UI) with basic localization.  
* **Database**: SQLite (local file-based, using Entity Framework Core **9.0**) for the Portal Backend.  
* **Mobile Data**: Static JSON file (recipes.json) exported from the Portal and embedded as MauiAsset in the MAUI application.  
* **Monetization**: Google AdMob (for banner and interstitial ads).

## **3\. Detailed Functionality**

### **3.1. Mobile App (.NET MAUI)**

The mobile application will operate entirely offline, reading its multi-language recipe and category data from the embedded recipes.json file. It's a read-only interface focused on user experience, content presentation, internationalization, and advertisement display.

**Language Management**:

* The application will maintain a global CurrentLanguage setting (e.g., "en" for English, "vi" for Vietnamese).  
* All UI text (static labels, buttons, navigation titles) will use .NET MAUI's built-in localization (.resx files).  
* All dynamic content (recipe names, descriptions, ingredients, instructions, **and category names**) will be loaded from the RecipeLocalizedText objects based on the CurrentLanguage.

**Screens (Pages):**

#### **3.1.1. Main Page (MainPage/HomePage)**

This will be the initial screen users see, displaying a list of available recipes in the selected language, organized by or filterable by category.

* **Layout**: Shell application with a FlyoutItem (optional, for future expansion) and a Tab or MainPage as the initial content.  
* **Header (Navigation Bar)**:  
  * **Title**: Dynamically loaded from .resx (e.g., AppResources.RecipeBookTitle).  
  * **Language Switcher**: A ToolbarItem (e.g., a globe icon or "EN/VI" text) that, when tapped, presents a simple choice (e.g., "English", "Tiếng Việt") via an ActionSheet or Popover. Changing the language will immediately update all dynamic and static text on the current page and subsequent pages.  
  * **Search Icon**: A ToolbarItem with an icon (e.g., a magnifying glass, ideally from MaterialDesignIcons or FontAwesome via a custom font) that, when tapped, reveals/hides a search bar.  
* **Search Functionality**:  
  * **Search Bar (SearchBar control)**: Initially hidden. When the search icon is tapped, it slides down/appears.  
  * **Placeholder**: Dynamically loaded from .resx (e.g., AppResources.SearchPlaceholder).  
  * **Behavior**: Filters the displayed recipes in real-time as the user types, matching against the **currently selected language's** Recipe Name and Ingredients (case-insensitive).  
* **Category Filter (Optional)**: A Picker or CollectionView of localized category names allowing users to filter the recipe list by a selected category. This could be a ToolbarItem dropdown or a separate UI element.  
* **Recipe List (CollectionView)**:  
  * **Data Source**: Binds to an ObservableCollection\<Recipe\> in the ViewModel, populated from recipes.json. The list should be filtered based on the selected language and optionally by category.  
  * **Layout**: VerticalList with GridItemsLayout for potentially multiple columns on tablets, or LinearItemsLayout for a single column on phones.  
  * **ItemTemplate (RecipeListItemView)**:  
    * **Layout**: A Frame or Border with CornerRadius="10" and Padding="15", possibly with Shadow effect, for each recipe.  
    * **Content**:  
      * **Image**: Image control, Source bound to ImageFileName (using an IValueConverter to map string filename to ImageSource.FromFile). Aspect="AspectFill", HeightRequest="150", WidthRequest="150" (or adaptive sizes). If ImageFileName is null/empty, use a placeholder image (e.g., https://placehold.co/150x150/cccccc/000000?text=No+Image).  
      * **Name**: Label, Text bound to Recipe.Name.GetLocalizedText(CurrentLanguage). FontAttributes="Bold", FontSize="Large", LineBreakMode="TailTruncation".  
      * **Category Name**: Label, Text bound to Recipe.Category.Name.GetLocalizedText(CurrentLanguage). FontSize="Small", TextColor="DarkCyan".  
      * **Description**: Label, Text bound to Recipe.Description.GetLocalizedText(CurrentLanguage). FontSize="Small", TextColor="Gray", MaxLines="2", LineBreakMode="TailTruncation".  
      * **Time**: Label, Text showing "Prep: {PrepTime} | Cook: {CookTime}". FontSize="Micro", TextColor="DarkGray". (PrepTime/CookTime remain non-localized as they are numerical values, but their labels "Prep:" and "Cook:" are localized).  
    * **Tap Gesture**: TapGestureRecognizer on the Frame/Border to navigate to RecipeDetailPage, passing the Recipe.Id as a parameter.  
* **Advertising (Google AdMob)**:  
  * **Banner Ad**: A **banner ad** should be displayed at the bottom of the MainPage (e.g., within a Grid.Row or VerticalStackLayout). This ad should be static and present throughout the page.  
* **ViewModel (MainViewModel.cs)**:  
  * ObservableCollection\<Recipe\> Recipes { get; set; } (Holds all recipes)  
  * ObservableCollection\<Recipe\> FilteredRecipes { get; set; } (Bound to CollectionView)  
  * ObservableCollection\<Category\> Categories { get; set; } (Holds all categories for filtering)  
  * Category SelectedCategory { get; set; } (Bound to category filter, triggers filtering)  
  * string SearchText { get; set; } (Bound to SearchBar)  
  * string CurrentLanguage { get; set; } (Property to hold and set the current language, triggering UI updates).  
  * ICommand SearchCommand { get; } (Triggers filtering based on CurrentLanguage).  
  * ICommand GoToRecipeDetailCommand { get; } (Navigates to detail page).  
  * ICommand ChangeLanguageCommand { get; } (Updates CurrentLanguage and potentially refreshes the UI).  
  * **Initialization**: Loads recipes.json asynchronously upon construction. Deserializes using System.Text.Json. Sets initial CurrentLanguage.

#### **3.1.2. Recipe Detail Page (RecipeDetailPage)**

This page displays the full details of a selected recipe in the chosen language.

* **Header (Navigation Bar)**:  
  * **Title**: Bound to Recipe.Name.GetLocalizedText(CurrentLanguage).  
  * **Back Button**: Standard MAUI back navigation.  
* **Layout**: ScrollView containing a VerticalStackLayout to ensure all content is scrollable.  
* **Content**:  
  * **Main Image**: Image control at the top, Source bound to ImageFileName (using the same IValueConverter). Aspect="AspectFill", HeightRequest="250" (or adaptive). Use placeholder if image not found.  
  * **Recipe Name**: Label, Text bound to Recipe.Name.GetLocalizedText(CurrentLanguage). FontAttributes="Bold", FontSize="ExtraLarge", Margin="10,15,10,5".  
  * **Category Name**: Label, Text bound to Recipe.Category.Name.GetLocalizedText(CurrentLanguage). FontSize="Medium", TextColor="DarkCyan", Margin="10,0,10,5".  
  * **Description**: Label, Text bound to Recipe.Description.GetLocalizedText(CurrentLanguage). FontSize="Medium", Margin="10,0,10,10".  
  * **Time Section**: HorizontalStackLayout  
    * Label, Text loaded from .resx (e.g., AppResources.PrepTimeLabel), FontAttributes="Bold".  
    * Label, Text bound to PrepTime.  
    * Label, Text loaded from .resx (e.g., AppResources.CookTimeLabel), FontAttributes="Bold".  
    * Label, Text bound to CookTime.  
    * Margin="10,0,10,15", FontSize="Small".  
  * **Ingredients Section**:  
    * Label, Text loaded from .resx (e.g., AppResources.IngredientsLabel), FontAttributes="Bold", FontSize="Large", Margin="10,0,10,5".  
    * CollectionView or VerticalStackLayout with individual Labels for each ingredient.  
    * **ItemTemplate (for Ingredients)**: Label, Text bound to IngredientLocalizedText.GetLocalizedText(CurrentLanguage), prefixed with a bullet point (e.g., "• {ingredient}"). FontSize="Medium", Margin="10,2".  
  * **Instructions Section**:  
    * Label, Text loaded from .resx (e.g., AppResources.InstructionsLabel), FontAttributes="Bold", FontSize="Large", Margin="10,15,10,5".  
    * CollectionView or VerticalStackLayout with individual Labels for each instruction step.  
    * **ItemTemplate (for Instructions)**: Label, Text bound to InstructionLocalizedText.GetLocalizedText(CurrentLanguage), prefixed with a step number (e.g., "1. {instruction}"). FontSize="Medium", Margin="10,2".  
* **Text Formatting (for Description, Ingredients, Instructions)**:  
  * For Description, PrepTime, CookTime, Name in detail page, use standard Label properties.  
  * For Ingredients and Instructions lists where individual items might have bold/italic content, use FormattedString for each Label if the content needs to be parsed (e.g., \*\*bold\*\* becomes bold, \*italic\* becomes italic). An IValueConverter or helper method can parse List\<string\> items into FormattedString objects.  
  * Alternatively, if Markdown.Forms.Maui is used, each item would be rendered by \<md:MarkdownView Markdown="{Binding .}" /\>.  
* **Advertising (Google AdMob)**:  
  * **Interstitial Ad**: An **interstitial (full-screen) ad** should be displayed when the user navigates *back* from the RecipeDetailPage to the MainPage (e.g., by handling the NavigatedFrom event or overriding the back button behavior). The ad should only appear intermittently, not on every return.  
* **ViewModel (RecipeDetailViewModel.cs)**:  
  * Recipe CurrentRecipe { get; set; } (Bound to the page content)  
  * string CurrentLanguage { get; set; } (Property to observe global language changes).  
  * **Initialization**: Receives the Recipe.Id as a navigation parameter, then finds the corresponding recipe from the loaded recipes.json data. Observes changes to CurrentLanguage and updates displayed text accordingly.

**RecipeLocalizedText Data Structure (Shared for all localized text fields)**:

This class should contain two string properties: English and Vietnamese. It should also provide a helper method, GetLocalizedText(string languageCode), which returns the appropriate language string based on the languageCode provided. If the languageCode is unknown, it should default to English. It should also handle null references for the strings.

**Category Data Structure (NEW \- for both mobile and portal)**:

This class represents a recipe category. It should have the following properties:

* Id (Guid): A unique identifier for each category.  
* Name (RecipeLocalizedText): The localized name of the category.

**Recipe Data Structure (for both mobile and portal \- UPDATED for localization and categories):**

This class represents a single recipe. It should have the following properties:

* Id (Guid): A unique identifier for each recipe.  
* Name (RecipeLocalizedText): The localized name of the recipe.  
* Description (RecipeLocalizedText): The localized short description of the recipe.  
* PrepTime (string): Preparation time, e.g., "10 minutes" (numerical, not localized).  
* CookTime (string): Cooking time, e.g., "15 minutes" (numerical, not localized).  
* ImageFileName (string): The name of the image file (e.g., "garlic\_butter\_bread.jpg") located in Resources/Raw/Images.  
* CategoryId (Guid): Foreign key to the Category this recipe belongs to.  
* Category (Category \- *for Portal Backend EF Core navigation only; in Mobile App JSON, the Category object will be embedded directly within Recipe*): The associated Category object.  
* Ingredients (List\<RecipeLocalizedText\>): A list of localized ingredients.  
* Instructions (List\<RecipeLocalizedText\>): A list of localized instruction steps.

**Note:** The ImageFileName property in the Recipe class will only contain the image's file name (e.g., garlic\_butter\_bread.jpg). These image files **must be manually placed** by the developer into the Resources/Raw/Images folder of the .NET MAUI project. This ensures they are embedded as MauiAsset resources and can be loaded directly by ImageSource.FromFile().

### **3.2. Portal Backend (ASP.NET Aspire)**

The Portal Backend will be an ASP.NET Core Web API project, orchestrated by ASP.NET Aspire, providing a RESTful API for multi-language recipe and category management, and a crucial export function.

* **Aspire AppHost Project (RecipeApp.AppHost)**:  
  * Configures and launches the RecipeApi.ApiService and RecipePortal.WebApp projects.  
  * Handles service discovery and environment variable injection (e.g., database connection strings).  
* **API Service Project (RecipeApi.ApiService)**:  
  * **Dependencies**: Microsoft.EntityFrameworkCore.Sqlite, Microsoft.AspNetCore.OpenApi, Swashbuckle.AspNetCore.  
  * **Data Models**: The RecipeLocalizedText, Category, and updated Recipe classes as defined above, used for data transfer objects (DTOs) and Entity Framework entities.  
  * AppDbContext.cs:  
    This class should inherit from DbContext and contain DbSet\<Recipe\> and DbSet\<Category\>.  
    It needs to configure how complex types like RecipeLocalizedText (for Name and Description in both Recipe and Category) and List\<RecipeLocalizedText\> (for Ingredients and Instructions in Recipe) properties are stored as JSON strings in SQLite. This will involve using Entity Framework Core's HasConversion method, System.Text.Json for serialization/deserialization, and custom ValueComparer for proper change tracking. A helper class for comparing RecipeLocalizedText objects within lists will also be needed.  
    Additionally, it needs to define the relationship between Recipe and Category (one-to-many, Recipe has one Category).  
  * **Program.cs**:  
    * Configures AddDbContext with SQLite.  
    * Applies migrations on startup (using a custom extension method for IApplicationBuilder like app.MigrateDatabase()).  
    * Adds Swagger/OpenAPI support for API documentation.  
  * **CategoriesController.cs (NEW API Controller)**:  
    * **GET /api/categories**: Returns List\<Category\>. Fetches all categories.  
    * **GET /api/categories/{id}**: Returns Category by Guid id. Fetches a single category. Returns 404 if not found.  
    * **POST /api/categories**: Creates a new Category. Accepts Category in the request body. Assigns new Guid.NewGuid() to Id. Saves to database. Returns 201 Created.  
    * **PUT /api/categories/{id}**: Updates an existing Category. Accepts Guid id and Category in the request body. Finds existing category. Updates its properties. Returns 204 No Content.  
    * **DELETE /api/categories/{id}**: Deletes a Category. Accepts Guid id. Removes from database. Returns 204 No Content. Returns 404 if not found.  
  * **RecipesController.cs (API Controller \- UPDATED)**:  
    * **GET /api/recipes**: Returns List\<Recipe\>. Fetches all recipes, **including their associated Category data (e.g., using .Include(r \=\> r.Category))**.  
    * **GET /api/recipes/{id}**: Returns Recipe by Guid id. Fetches a single recipe, **including its associated Category data**. Returns 404 if not found.  
    * **POST /api/recipes**: Creates a new Recipe. Accepts Recipe in the request body. Assigns new Guid.NewGuid() to Id. Ensures CategoryId is valid. Saves to database. Returns 201 Created with the new recipe.  
    * **PUT /api/recipes/{id}**: Updates an existing Recipe. Accepts Guid id and Recipe in the request body. Finds existing recipe. Updates its properties, including CategoryId. Returns 204 No Content or the updated recipe. Returns 404 if not found.  
    * **DELETE /api/recipes/{id}**: Deletes a Recipe. Accepts Guid id. Removes from database. Returns 204 No Content. Returns 404 if not found.  
    * **GET /api/recipes/export**: **Crucial Export Endpoint.** Fetches *all* recipes from \_context.Recipes, **including their associated Category data**. Serializes the List\<Recipe\> to a JSON string using System.Text.Json (JsonSerializerOptions { WriteIndented \= true }). Returns the JSON string as application/json content type, with a Content-Disposition header to prompt file download (filename recipes.json). Ensure that the exported JSON for Recipe objects directly embeds the Category object, rather than just CategoryId, for the MAUI app's offline use.  
* **Database Setup**:  
  * Ensure Entity Framework Core migrations are properly configured (e.g., dotnet ef migrations add InitialCreate, dotnet ef database update). The new Category entity and the relationship to Recipe must be included in migrations.  
  * The SQLite database file (e.g., recipes.db) will reside in the API service project's directory or a configured location.

### **3.3. Portal Frontend (Blazor)**

The Blazor WebAssembly application will provide the administrative interface for multi-language content authors. It will interact with the Portal Backend's API.

* **Blazor WebApp Project (RecipePortal.WebApp)**:  
  * **Dependencies**: Microsoft.AspNetCore.Components.WebAssembly.Authentication (if auth is ever considered), System.Net.Http.Json.  
  * **Data Models**: The RecipeLocalizedText, Category, and updated Recipe classes.  
  * **Shared Layout**: A basic layout with navigation links to "Recipe List", "**Category List**", and potentially an "About" page. It will also include a simple language switcher (e.g., dropdown) for the portal's UI, using Blazor's built-in localization or a custom approach.  
  * **Tailwind CSS Integration**: Configure Tailwind CSS for styling.  
* **Screens (Pages):**

#### **3.3.1. Category List (Pages/CategoryList.razor \- NEW)**

* **Layout**: A central container with a header, "Add New Category" button, and the category list.  
* **Header**: h1 tag "Category Management" (localized in Blazor UI).  
* **Action Buttons**:  
  * **"Add New Category" Button**: Navigates to /category/new. Styled as a primary button.  
* **Category List (Table or div based layout)**:  
  * Displays categories. Each row/card represents a category.  
  * **Content per entry**: Category Name (e.g., "Name (EN)", "Name (VI)").  
  * **Action Buttons per entry**:  
    * **"Edit" Button**: Navigates to /category/edit/{id}.  
    * **"Delete" Button**:  
      * Triggers a confirmation dialog (custom modal).  
      * If confirmed, calls HttpClient.DeleteAsync($"/api/categories/{id}").  
      * Refreshes the category list. **Important: Consider preventing deletion if category is associated with existing recipes, or implement cascade delete logic.**

#### **3.3.2. Add/Edit Category (Pages/CategoryEdit.razor \- NEW)**

* **Layout**: A form layout for entering/editing multi-language category details.  
* **Header**: h1 tag, dynamically displays "Add New Category" or "Edit Category" (localized).  
* **Input Form**:  
  * **Name**:  
    * Name.English: \<InputText @bind-Value="Category.Name.English" /\> (Label: "Category Name (English)")  
    * Name.Vietnamese: \<InputText @bind-Value="Category.Name.Vietnamese" /\> (Label: "Tên danh mục (Tiếng Việt)")  
* **Validation**: Use EditForm and DataAnnotationsValidator. Ensure validation messages are localized.  
* **Action Buttons**:  
  * **"Save" Button**: Validates the form. Calls HttpClient.PostAsJsonAsync or HttpClient.PutAsJsonAsync to /api/categories. Navigates back to CategoryList on success.  
  * **"Cancel" Button**: Navigates back to CategoryList without saving.

#### **3.3.3. Recipe List (Pages/RecipeList.razor \- UPDATED)**

* **Layout**: A central container with a header, action buttons, and the recipe list.  
* **Header**: h1 tag "Recipe Management" (localized in Blazor UI).  
* **Action Buttons**:  
  * **"Add New Recipe" Button**: Navigates to /recipe/new. Styled as a primary button.  
  * **"Export JSON" Button**:  
    * Styled as a secondary action button.  
    * When clicked, it calls HttpClient.GetAsync("/api/recipes/export").  
    * Upon successful response, it initiates a file download for recipes.json using JavaScript interop (IJSRuntime).  
* **Recipe List (Table or div based layout)**:  
  * Displays recipes. Each row/card represents a recipe.  
  * **Content per entry**: Recipe Name (e.g., "Name (EN)", "Name (VI)"), Category Name (e.g., "Category (EN)"), Description (truncated).  
  * **Action Buttons per entry**:  
    * **"Edit" Button**: Navigates to /recipe/edit/{id}.  
    * **"Delete" Button**:  
      * Triggers a confirmation dialog (custom modal).  
      * If confirmed, calls HttpClient.DeleteAsync($"/api/recipes/{id}").  
      * Refreshes the recipe list.

#### **3.3.4. Add/Edit Recipe (Pages/RecipeEdit.razor \- UPDATED)**

* **Layout**: A form layout for entering/editing multi-language recipe details.  
* **Header**: h1 tag, dynamically displays "Add New Recipe" or "Edit Recipe" (localized).  
* **Input Form**:  
  * **Name**:  
    * Name.English: \<InputText @bind-Value="Recipe.Name.English" /\> (Label: "Recipe Name (English)")  
    * Name.Vietnamese: \<InputText @bind-Value="Recipe.Name.Vietnamese" /\> (Label: "Tên công thức (Tiếng Việt)")  
  * **Description**:  
    * Description.English: \<InputTextArea @bind-Value="Recipe.Description.English" /\> (Label: "Description (English)")  
    * Description.Vietnamese: \<InputTextArea @bind-Value="Recipe.Description.Vietnamese" /\> (Label: "Mô tả (Tiếng Việt)")  
  * **PrepTime**: \<InputText @bind-Value="Recipe.PrepTime" /\> (Label: "Preparation Time" / "Thời gian chuẩn bị" \- localized label, value not localized)  
  * **CookTime**: \<InputText @bind-Value="Recipe.CookTime" /\> (Label: "Cook Time" / "Thời gian nấu" \- localized label, value not localized)  
  * **ImageFileName**: \<InputText @bind-Value="Recipe.ImageFileName" /\> (Label: "Image File Name (e.g., dish.jpg)", Placeholder: "garlic\_butter\_bread.jpg") \- Include a tooltip explaining to place images in MAUI project.  
  * **Category Selector**:  
    * A select element or custom dropdown component bound to Recipe.CategoryId.  
    * Populates with a list of categories fetched from /api/categories.  
    * Displays localized category names (e.g., "Name (EN)" or "Tên danh mục (VI)") in the dropdown.  
    * Label: "Category" / "Danh mục" (localized).  
  * **Ingredients**:  
    * IngredientsTextEnglish: \<InputTextArea @bind-Value="IngredientsTextEnglish" /\> (Label: "Ingredients (English, one per line)")  
    * IngredientsTextVietnamese: \<InputTextArea @bind-Value="IngredientsTextVietnamese" /\> (Label: "Nguyên liệu (Tiếng Việt, mỗi dòng một nguyên liệu)")  
    * Helper properties (IngredientsTextEnglish, IngredientsTextVietnamese) convert List\<RecipeLocalizedText\> to/from multiline strings.  
  * **Instructions**:  
    * InstructionsTextEnglish: \<InputTextArea @bind-Value="InstructionsTextEnglish" /\> (Label: "Instructions (English, one step per line)")  
    * InstructionsTextVietnamese: \<InputTextArea @bind-Value="InstructionsTextVietnamese" /\> (Label: "Hướng dẫn (Tiếng Việt, mỗi dòng một bước)")  
    * Helper properties (InstructionsTextEnglish, InstructionsTextVietnamese) convert List\<RecipeLocalizedText\> to/from multiline strings.  
* **Validation**: Use EditForm and DataAnnotationsValidator. Ensure validation messages are localized in the Blazor UI.  
* **Action Buttons**:  
  * **"Save" Button**: Validates the form. Calls HttpClient.PostAsJsonAsync or HttpClient.PutAsJsonAsync. Navigates back to RecipeList on success.  
  * **"Cancel" Button**: Navigates back to RecipeList without saving.

## **4\. Development Workflow**

1. **Start Aspire AppHost**: In Visual Studio or via dotnet run in the RecipeApp.AppHost project, launch the entire solution. Aspire will automatically start and orchestrate the API Backend and Blazor Frontend. Access the Aspire Dashboard to monitor services.  
2. **Manage Multi-Language Categories via Blazor Portal**: First, use the new "Category List" and "Add/Edit Category" pages in the Blazor Frontend to define and manage your recipe categories, ensuring both English and Vietnamese names are provided.  
3. **Manage Multi-Language Recipes via Blazor Portal**: Then, use the "Recipe List" and "Add/Edit Recipe" pages to populate and refine your multi-language recipe database. Ensure both English and Vietnamese fields are filled for each recipe, and select an appropriate category from the dropdown. All data is saved to the local SQLite database.  
4. **Export JSON for Mobile App**: Once multi-language content (recipes and categories) is ready for the mobile app, navigate to the "Recipe List" page in the Blazor Portal and click the "Export JSON" button. This will download the recipes.json file containing all localized text and embedded category data.  
5. **Update Mobile App Assets**:  
   * Place the downloaded recipes.json file into the Resources/Raw folder of your .NET MAUI project. Ensure its Build Action is set to MauiAsset.  
   * Manually copy any referenced image files (e.g., garlic\_butter\_bread.jpg) into the Resources/Raw/Images folder within the .NET MAUI project. Ensure their Build Action is also MauiAsset.  
   * Update .resx files in the MAUI project with all static UI strings for both English and Vietnamese.  
6. **Build and Run Mobile App**: Compile and run the .NET MAUI application on your target emulator/device. It will now display the latest content from the recipes.json file, **in the user-selected language, with recipes categorized and along with AdMob advertisements**.

## **5\. UI/UX Requirements**

* **Consistent UI/UX**:  
  * **Blazor Frontend**: Utilize **Tailwind CSS** extensively for all styling. Ensure forms are well-aligned, buttons have clear states, and data tables are readable. Labels, messages, and button texts should be localized within the Blazor UI itself.  
  * **.NET MAUI Mobile App**: Leverage the default MAUI controls and styling capabilities. Focus on clean layouts, readable typography (e.g., default "Inter" font or system font), and intuitive navigation. Use Style definitions in App.xaml for consistent element appearance. All static and dynamic text must be localized based on CurrentLanguage.  
* **User-Friendly**: Interfaces should be intuitive for both content authors (Blazor) and end-users (MAUI). Clear labels, helpful placeholders, and logical information flow are essential. The language switching mechanism must be easy to find and use. Category management should be straightforward.  
* **Responsive Design**:  
  * **Blazor Frontend**: The portal should adapt gracefully to various screen sizes (desktop, tablet) using Tailwind's responsive utility classes.  
  * **.NET MAUI Mobile App**: Design layouts to be fully responsive, ensuring optimal viewing and usability on all devices and orientations. Use MAUI's Grid, FlexLayout, and relative sizing for adaptability. Avoid fixed pixel values where possible.  
* **Feedback & Error Handling**: Implement user feedback mechanisms (e.g., toast messages for success/failure in Blazor, simple error pop-ups in MAUI for asset loading failures). Use custom modal dialogs for confirmations instead of browser alerts.

## **6\. Initial Implementation Steps for AI Tool**

To kickstart the project, the AI tool should proceed with the following steps:

1. **Initialize Aspire Solution**:  
   * Create a new .NET solution using the Aspire AppHost template.  
   * Add an ASP.NET Core Web API project named RecipeApi.ApiService to the Aspire solution.  
   * Add a Blazor WebAssembly (or Blazor Server for simplicity) project named RecipePortal.WebApp to the Aspire solution.  
   * Reference RecipeApi.ApiService from RecipePortal.WebApp for API access and from RecipeApp.AppHost for orchestration.  
   * Reference RecipePortal.WebApp from RecipeApp.AppHost for orchestration.  
2. **Define Shared Data Models (Recipe.cs, RecipeLocalizedText.cs, Category.cs)**:  
   * Create the RecipeLocalizedText, Category, and updated Recipe classes in RecipeApi.ApiService that match the specified multi-language data structure, including the relationship between Recipe and Category. These classes will serve as both the EF Core entities and the DTOs for API/Blazor.  
3. **Configure Entity Framework Core & Database**:  
   * In RecipeApi.ApiService, create an AppDbContext.cs class inheriting from DbContext, containing DbSet\<Recipe\> and DbSet\<Category\>.  
   * Implement the OnModelCreating method in AppDbContext to configure how RecipeLocalizedText (for Name and Description in both Recipe and Category) and List\<RecipeLocalizedText\> (for Ingredients and Instructions in Recipe) properties are stored as JSON strings in SQLite. This will involve using Entity Framework Core's HasConversion method, System.Text.Json for serialization/deserialization, and custom ValueComparer for proper change tracking. A helper class for comparing RecipeLocalizedText objects within lists will also be needed.  
   * Configure the one-to-many relationship between Recipe and Category in OnModelCreating.  
   * Configure SQLite as the database provider in Program.cs of RecipeApi.ApiService.  
   * Add initial EF Core migrations (e.g., InitialCreate) and ensure database migrations are applied on startup.  
4. **Implement API Controllers (RecipesController.cs, CategoriesController.cs)**:  
   * In RecipeApi.ApiService, create CategoriesController.cs with ApiController attribute and implement all specified CRUD (GET, POST, PUT, DELETE) endpoints for Category entities.  
   * Update RecipesController.cs with ApiController attribute. Implement all specified CRUD (GET, POST, PUT, DELETE) endpoints for Recipe entities. Ensure GET operations eager load (.Include()) the associated Category data.  
   * Crucially, implement the GET /api/recipes/export endpoint to fetch *all* recipes (including Category data), serialize them as a List\<Recipe\> where Category is embedded, and return them as a downloadable recipes.json file.  
5. **Set Up Blazor Frontend (RecipePortal.WebApp)**:  
   * Integrate Tailwind CSS into the Blazor project.  
   * Implement basic localization for static UI elements in the Blazor app.  
   * Create Pages/CategoryList.razor to display and manage categories.  
   * Create Pages/CategoryEdit.razor as a multi-language form to add/edit category details.  
   * Update Pages/RecipeList.razor to display recipe details including the category name.  
   * Update Pages/RecipeEdit.razor as a multi-language form to add/edit recipe details, including a dropdown selector for categories. Implement helper properties for List\<RecipeLocalizedText\> to handle multiline string input for ingredients and instructions.  
   * Implement client-side validation for all forms, ensuring validation messages are localized.  
   * Implement JavaScript interop for the file download functionality of the "Export JSON" button.  
6. **Set Up .NET MAUI Project (RecipeApp.Mobile)**:  
   * Create a new .NET MAUI project named RecipeApp.Mobile.  
   * Copy the Recipe.cs, RecipeLocalizedText.cs, and Category.cs classes into the MAUI project. Add the GetLocalizedText extension method to RecipeLocalizedText.  
   * Create Resources/Raw and Resources/Raw/Images folders within the MAUI project.  
   * Create .resx files (e.g., AppResources.resx, AppResources.vi.resx) for static UI localization.  
   * Implement a global language service or singleton to manage CurrentLanguage and notify UI components of changes.  
   * Implement MainPage.xaml and its corresponding ViewModel (MainViewModel.cs) to display the recipe list (CollectionView), implement search functionality (based on CurrentLanguage), include the language switcher, and optionally implement category filtering.  
   * Implement RecipeDetailPage.xaml and its ViewModel (RecipeDetailViewModel.cs) to display full recipe details, with all dynamic content (including category name) bound to GetLocalizedText(CurrentLanguage).  
   * Implement an IValueConverter to convert ImageFileName string to ImageSource.FromFile().  
   * Ensure proper navigation between MainPage and RecipeDetailPage.  
7. **Integrate Google AdMob (MAUI)**:  
   * **Research & Install AdMob SDK**: The AI tool should research the **latest official Google AdMob SDK for .NET MAUI (or Xamarin.Forms, which can be adapted)**. A common approach involves using **Xamarin.Firebase.iOS.AdMob** and **Xamarin.Firebase.Messaging** (for Android) packages or community-maintained wrappers, along with custom renderers or platform-specific implementations.  
   * **NuGet Packages**: Install necessary NuGet packages. For Android: Xamarin.Firebase.Messaging and Xamarin.Firebase.Ads. For iOS: Xamarin.Firebase.iOS.AdMob.  
   * **Platform-Specific Setup**:  
     * **Android**:  
       * Update AndroidManifest.xml: Add \<meta-data android:name="com.google.android.gms.ads.APPLICATION\_ID" android:value="ca-app-pub-xxxxxxxxxxxxxxxx\~yyyyyyyyyy"/\> within the \<application\> tag (replace with your actual AdMob App ID).  
       * Initialize Mobile Ads SDK in MainActivity.cs (e.g., MobileAds.Initialize(this);).  
     * **iOS**:  
       * Update Info.plist: Add GADApplicationIdentifier entry with your AdMob App ID.  
       * Initialize Mobile Ads SDK in AppDelegate.cs (e.g., MobileAds.SharedInstance.Start(CompletionHandler);).  
   * **Custom Ad View Controls**:  
     * Create custom MAUI AdView controls (e.g., AdMobBannerView.cs, AdMobInterstitialAd.cs) that act as placeholders.  
     * Implement **Platform-Specific Renderers/Handlers** for Android and iOS (e.g., using PlatformView in .NET MAUI handlers) to display native AdMob banner ads.  
   * **Banner Ad Implementation**:  
     * In MainPage.xaml, place the custom AdMobBannerView control at the specified bottom position, binding its AdUnitId property to a **placeholder Ad Unit ID** (e.g., ca-app-pub-xxxxxxxxxxxxxxxx/bbbbbbbbbb).  
   * **Interstitial Ad Implementation**:  
     * In RecipeDetailViewModel.cs or RecipeDetailPage.xaml.cs, implement logic to **load and display an interstitial ad** when the user navigates back from the detail page. This will involve creating an instance of AdMobInterstitialAd and calling its LoadAd() and ShowAd() methods. Bind its AdUnitId property to a **placeholder Interstitial Ad Unit ID** (e.g., ca-app-pub-xxxxxxxxxxxxxxxx/iiiiiiiiii).  
     * Implement a mechanism to control the frequency of interstitial ads (e.g., show only every N times, or after a certain delay).  
   * **Ad Unit IDs**: Use **Google's test Ad Unit IDs** during development to avoid invalid clicks on real ads. Replace these with your actual Ad Unit IDs before deploying to app stores.  
     * Test Banner Ad Unit ID (Android): ca-app-pub-3940256099942544/6300978111  
     * Test Banner Ad Unit ID (iOS): ca-app-pub-3940256099942544/2934735716  
     * Test Interstitial Ad Unit ID (Android): ca-app-pub-3940256099942544/1033173712  
     * Test Interstitial Ad Unit ID (iOS): ca-app-pub-3940256099942544/4414689103  
   * **Remember to add your actual AdMob Application ID and Ad Unit IDs before publishing.**  
8. **Styling**: Apply basic yet appealing styling to both Blazor (using Tailwind CSS) and MAUI (using MAUI's built-in styling and resource dictionaries) components as described in the UI/UX requirements.