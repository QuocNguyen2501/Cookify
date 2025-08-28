# Playwright MCP Testing Instructions for Cookify WebApp

## 🎯 Overview

This guide provides comprehensive testing instructions for the Cookify Recipe Portal WebApp using Playwright MCP. The WebApp is a Blazor WebAssembly application that provides an administrative interface for managing multi-language recipes and categories.

## 🏗️ Application Architecture

### Core Components
- **Technology**: Blazor WebAssembly with .NET 9.0
- **Styling**: Tailwind CSS utility classes
- **Backend**: ASP.NET Core API via HTTP client
- **Data**: Multi-language support (English/Vietnamese) using `RecipeLocalizedText`

### Key Pages & URLs
- **Home**: `/` - Welcome page
- **Categories**: `/categories` - Category management list
- **Category Edit**: `/category/new`, `/category/edit/{id}` - Add/edit categories
- **Recipes**: `/recipes` - Recipe management list with export functionality
- **Recipe Edit**: `/recipe/new`, `/recipe/edit/{id}` - Add/edit recipes

## 🚀 Prerequisites & Setup

### Before Testing
1. **Start Application**: Ensure the application is running via Aspire
   ```bash
   dotnet run --project RecipeApp.AppHost
   ```
2. **Base URL**: Default is `https://localhost:7042` or check Aspire dashboard
3. **Database**: SQLite database should be seeded with sample data on first run

### MCP Browser Setup
The Playwright MCP uses specific tool names that differ from my initial examples. The correct usage is:

**Important**: Use the exact tool names as defined in the MCP:
- `browser_navigate` (not `mcp_playwright_browser_navigate`)
- `browser_click` (not `mcp_playwright_browser_click`)
- `browser_snapshot` (not `mcp_playwright_browser_snapshot`)
- etc.

**Key Differences from Standard Playwright**:
- **Element References**: Use `ref` parameter with element IDs from snapshots, not CSS selectors
- **Snapshots First**: Always take `browser_snapshot()` to see element references before interacting
- **Human-readable Descriptions**: Use descriptive `element` parameter alongside `ref`
- **No Await**: MCP tools are called directly, not with `await`

### Essential MCP Testing Workflow
```javascript
// 1. Navigate to page
browser_navigate({ url: "https://localhost:7042/categories" });

// 2. Take snapshot to see available elements and their refs
browser_snapshot();
// This will show elements like:
// - button "Add New Category" [ref=e3]
// - link "Edit" [ref=e5] 

// 3. Interact with elements using the ref from snapshot
browser_click({
    element: "Add New Category button",
    ref: "e3"  // Use the exact ref from snapshot
});

// 4. Take another snapshot to verify the action
browser_snapshot();
```

**Critical**: Always take a snapshot before interacting with elements to get the correct `ref` values.

## 📋 Testing Scenarios

### 1. Navigation & Layout Testing

#### 1.1 Main Layout Verification
```javascript
// Take a snapshot to verify main navigation elements
browser_snapshot()
// Check for:
// - "Recipe Portal" brand text
// - Home navigation link
// - Categories navigation link  
// - Recipes navigation link
```

#### 1.2 Navigation Flow Testing
```javascript
// Test navigation to Categories
browser_click({
    element: "Categories navigation link",
    ref: "link_to_categories_page"
});

// Verify URL change by taking snapshot
browser_snapshot()
// Expected: URL shows /categories

// Test navigation to Recipes
browser_click({
    element: "Recipes navigation link", 
    ref: "link_to_recipes_page"
});

// Verify URL change
browser_snapshot()
// Expected: URL shows /recipes
```

### 2. Category Management Testing

#### 2.1 Category List Page Testing
```javascript
// Navigate to categories page
browser_navigate({ url: "https://localhost:7042/categories" });

// Take snapshot to verify page structure
browser_snapshot();

// Verify key elements:
// - Page title "Category Management"
// - "Add New Category" button
// - Categories table with columns: English Name, Vietnamese Name, Actions
// - Sample data: "Main Dishes", "Desserts" etc.
```

#### 2.2 Add New Category Testing
```javascript
// Click Add New Category button
browser_click({
    element: "Add New Category button",
    ref: "add_new_category_button"
});

// Verify navigation to /category/new by taking snapshot
browser_snapshot();

// Fill out the form using browser_fill_form
browser_fill_form({
    fields: [
        {
            name: "English Name",
            type: "textbox",
            ref: "englishName_input",
            value: "Test Category EN"
        },
        {
            name: "Vietnamese Name", 
            type: "textbox",
            ref: "vietnameseName_input",
            value: "Test Category VI"
        }
    ]
});

// Submit the form
browser_click({
    element: "Save Category button",
    ref: "save_category_button"
});

// Verify redirect back to categories list
browser_snapshot();
// Check for success message or new category in list
```

#### 2.3 Edit Category Testing
```javascript
// Navigate to categories list
browser_navigate({ url: "https://localhost:7042/categories" });

// Click edit link for first category
browser_click({
    element: "Edit button for first category",
    ref: "first_category_edit_button"
});

// Modify the category name using browser_type
browser_type({
    element: "English Name field",
    ref: "englishName_input",
    text: "Modified Category Name"
});

// Save changes
browser_click({
    element: "Save Category button", 
    ref: "save_category_button"
});

// Verify changes are saved
browser_snapshot();
```

#### 2.4 Delete Category Testing
```javascript
// Navigate to categories list
browser_navigate({ url: "https://localhost:7042/categories" });

// Click delete button
browser_click({
    element: "Delete button for category",
    ref: "category_delete_button"
});

// Handle confirmation dialog
browser_handle_dialog({
    accept: true
});

// Verify category is removed from list
browser_snapshot();
```

### 3. Recipe Management Testing

#### 3.1 Recipe List Page Testing
```javascript
// Navigate to recipes page
browser_navigate({ url: "https://localhost:7042/recipes" });

// Take snapshot to verify page structure
browser_snapshot();

// Verify key elements:
// - Page title "Recipe Management"
// - "Export JSON" button (green)
// - "Add New Recipe" button (blue)
// - Recipes table with columns: English Name, Vietnamese Name, Category, Prep/Cook Time, Actions
// - Sample recipes with Vietnamese names
```

#### 3.2 Export Functionality Testing
```javascript
// Test JSON export
browser_click({
    element: "Export JSON button",
    ref: "export_json_button"
});

// Wait for download to complete
browser_wait_for({ time: 2 });

// Take snapshot to verify export completion
browser_snapshot();
// Note: File download verification may require additional setup
```

#### 3.3 Add New Recipe Testing
```javascript
// Click Add New Recipe button
browser_click({
    element: "Add New Recipe button",
    ref: "add_new_recipe_button"
});

// Verify navigation to /recipe/new
browser_snapshot();

// Fill out comprehensive recipe form using browser_fill_form
browser_fill_form({
    fields: [
        {
            name: "English Name",
            type: "textbox", 
            ref: "englishName_input",
            value: "Test Recipe EN"
        },
        {
            name: "Vietnamese Name",
            type: "textbox",
            ref: "vietnameseName_input", 
            value: "Test Recipe VI"
        },
        {
            name: "English Description",
            type: "textbox",
            ref: "englishDescription_textarea",
            value: "Test description in English"
        },
        {
            name: "Vietnamese Description", 
            type: "textbox",
            ref: "vietnameseDescription_textarea",
            value: "Test description in Vietnamese"
        },
        {
            name: "Prep Time",
            type: "textbox",
            ref: "prepTime_input",
            value: "15 minutes"
        },
        {
            name: "Cook Time",
            type: "textbox",
            ref: "cookTime_input",
            value: "30 minutes"
        },
        {
            name: "Image File Name",
            type: "textbox",
            ref: "imageFileName_input", 
            value: "test_recipe.jpg"
        }
    ]
});

// Select category from dropdown
browser_select_option({
    element: "Category dropdown",
    ref: "category_select",
    values: ["first-category-guid"] // Use actual category ID from snapshot
});

// Add ingredients (dynamic sections)
browser_click({
    element: "Add Ingredient button",
    ref: "add_ingredient_button"
});

// Fill ingredient fields
browser_type({
    element: "First ingredient English field",
    ref: "ingredient_english_input_0",
    text: "Test Ingredient EN"
});

browser_type({
    element: "First ingredient Vietnamese field", 
    ref: "ingredient_vietnamese_input_0",
    text: "Test Ingredient VI"
});

// Add instructions (dynamic sections)
browser_click({
    element: "Add Instruction button",
    ref: "add_instruction_button"
});

// Fill instruction fields
browser_type({
    element: "First instruction English field",
    ref: "instruction_english_textarea_0", 
    text: "Test instruction in English"
});

browser_type({
    element: "First instruction Vietnamese field",
    ref: "instruction_vietnamese_textarea_0",
    text: "Test instruction in Vietnamese"
});

// Submit the form
browser_click({
    element: "Save Recipe button",
    ref: "save_recipe_button"
});

// Verify redirect and success
browser_snapshot();
```

#### 3.4 Edit Recipe Testing
```javascript
// Navigate to recipes list
browser_navigate({ url: "https://localhost:7042/recipes" });

// Click edit link for first recipe  
browser_click({
    element: "Edit button for first recipe",
    ref: "first_recipe_edit_button"
});

// Modify recipe data
browser_type({
    element: "English Name field",
    ref: "englishName_input",
    text: "Modified Recipe Name"
});

// Save changes
browser_click({
    element: "Save Recipe button",
    ref: "save_recipe_button"
});

// Verify changes are saved
browser_snapshot();
```

### 4. Form Validation Testing

#### 4.1 Required Field Validation
```javascript
// Navigate to add new recipe
browser_navigate({ url: "https://localhost:7042/recipe/new" });

// Try to submit empty form
browser_click({
    element: "Save Recipe button",
    ref: "save_recipe_button"
});

// Verify validation messages appear
browser_snapshot();
// Look for validation error messages for required fields
```

#### 4.2 Category Selection Validation
```javascript
// Fill partial form without category using browser_type
browser_type({
    element: "English Name field",
    ref: "englishName_input", 
    text: "Test Recipe"
});

// Submit without selecting category
browser_click({
    element: "Save Recipe button",
    ref: "save_recipe_button"
});

// Verify category validation error
browser_snapshot();
```

### 5. Multi-Language Content Verification

#### 5.1 Content Display Testing
```javascript
// Navigate to recipes list
browser_navigate({ url: "https://localhost:7042/recipes" });

// Take snapshot and verify both English and Vietnamese content is displayed
browser_snapshot();

// Verify:
// - English recipe names in first column
// - Vietnamese recipe names in second column
// - Both languages are properly populated
```

#### 5.2 Form Language Fields Testing
```javascript
// Navigate to recipe edit form
browser_navigate({ url: "https://localhost:7042/recipe/new" });

// Verify both language input fields are present
browser_snapshot();

// Check for:
// - English name/description fields
// - Vietnamese name/description fields  
// - Ingredient English/Vietnamese fields
// - Instruction English/Vietnamese fields
```

### 6. Responsive Design Testing

#### 6.1 Desktop Layout Testing
```javascript
// Set desktop viewport
browser_resize({ width: 1920, height: 1080 });

// Navigate through pages and verify layout
browser_navigate({ url: "https://localhost:7042/recipes" });
browser_snapshot(); // Desktop layout verification
```

#### 6.2 Tablet Layout Testing  
```javascript
// Set tablet viewport
browser_resize({ width: 768, height: 1024 });

// Test navigation and forms on tablet size
browser_navigate({ url: "https://localhost:7042/categories" });
browser_snapshot(); // Tablet layout verification
```

#### 6.3 Mobile Layout Testing
```javascript
// Set mobile viewport
browser_resize({ width: 375, height: 667 });

// Test mobile navigation and forms
browser_navigate({ url: "https://localhost:7042" });
browser_snapshot(); // Mobile layout verification
```

### 7. Error Handling Testing

#### 7.1 Network Error Testing
```javascript
// Simulate network issues by navigating to invalid endpoint
browser_navigate({ url: "https://localhost:7042/invalid-endpoint" });

// Verify error handling
browser_snapshot(); // Check for 404 or error page
```

#### 7.2 API Error Testing
```javascript
// Test form submission when API is unreachable
// (May require stopping the API service temporarily)
browser_navigate({ url: "https://localhost:7042/recipe/new" });

browser_fill_form({
    fields: [
        {
            name: "English Name",
            type: "textbox",
            ref: "englishName_input",
            value: "Test Recipe"
        }
    ]
});

browser_click({
    element: "Save Recipe button", 
    ref: "save_recipe_button"
});

// Verify error message display
browser_snapshot();
```

## 🔍 Key Testing Points

### Critical UI Elements to Verify
1. **Navigation Menu**: All links functional and styled correctly
2. **Tables**: Data loads correctly, columns aligned, responsive
3. **Forms**: All input fields accessible, validation working
4. **Buttons**: Proper styling (Tailwind classes), hover states
5. **Modal Dialogs**: Delete confirmations work properly
6. **Multi-language Fields**: Both English/Vietnamese inputs present

### Data Integrity Checks
1. **Category Dependencies**: Recipes properly reference categories
2. **Multi-language Consistency**: Both languages save and display
3. **Export Functionality**: JSON contains complete data structure
4. **Form Persistence**: Data survives page navigation

### Accessibility Testing
1. **Form Labels**: All inputs have proper labels
2. **Keyboard Navigation**: Tab through forms works
3. **Screen Reader**: Proper ARIA attributes where needed
4. **Color Contrast**: Tailwind classes meet accessibility standards

## 🚨 Common Issues to Watch For

### Known Challenges
1. **Blazor Loading**: Pages may show "Loading..." state briefly
2. **Dynamic Forms**: Ingredient/instruction sections added dynamically
3. **Category Dependencies**: Cannot delete categories with associated recipes
4. **File Downloads**: Export functionality triggers browser download
5. **Form Validation**: Client-side validation via DataAnnotations

### Debugging Tips
1. **Console Errors**: Check browser console for JavaScript errors
2. **Network Tab**: Verify API calls are successful (200 status)
3. **Blazor Reconnection**: Watch for SignalR reconnection messages
4. **Loading States**: Allow time for async operations to complete

## 📊 Test Results Documentation

### Expected Outcomes
- All navigation links work correctly
- CRUD operations complete successfully  
- Forms validate properly
- Multi-language content displays correctly
- Export functionality downloads JSON file
- Responsive design works across viewport sizes

### Success Criteria
- ✅ No console errors during navigation
- ✅ All forms submit successfully with valid data
- ✅ Validation prevents invalid submissions
- ✅ Data persists correctly between page loads
- ✅ Multi-language content displays in correct columns
- ✅ Export generates valid JSON with embedded category data

---

**Note**: This is a comprehensive testing guide for the complete Cookify WebApp. The application uses Tailwind CSS for styling and follows Blazor WebAssembly patterns with proper form validation and multi-language support.
