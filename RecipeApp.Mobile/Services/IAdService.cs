namespace RecipeApp.Mobile.Services;

public interface IAdService
{
    /// <summary>
    /// Loads an interstitial ad for later display
    /// </summary>
    Task LoadInterstitialAdAsync();

    /// <summary>
    /// Shows the loaded interstitial ad if available
    /// </summary>
    Task ShowInterstitialAdAsync();

    /// <summary>
    /// Checks if an interstitial ad is ready to be shown
    /// </summary>
    bool IsInterstitialAdReady { get; }
}
