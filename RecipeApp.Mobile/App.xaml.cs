namespace RecipeApp.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var services = IPlatformApplication.Current?.Services;
		var appShell = services?.GetService<AppShell>();
		return new Window(appShell ?? throw new InvalidOperationException("Could not resolve AppShell from DI"));
	}
}
