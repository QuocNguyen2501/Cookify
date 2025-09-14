using RecipePortal.WebApp;
using RecipePortal.WebApp.Components;
using RecipePortal.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

// Add HttpClient for API calls - Use Aspire service discovery properly
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new("https+http://apiservice");
    // Set timeout to 65 minutes to be slightly longer than API timeout (60 minutes)
    client.Timeout = TimeSpan.FromMinutes(65);
});

// Register the named client as the default HttpClient for direct injection
builder.Services.AddScoped(sp => 
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return httpClientFactory.CreateClient("ApiClient");
});

// Register Recipe Form Service
builder.Services.AddScoped<IRecipeFormService, RecipeFormService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
