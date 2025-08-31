using Plugin.MauiMTAdmob;

namespace RecipeApp.Mobile.Services;

public class AdService : IAdService
{
    // Demo Interstitial Ad Unit ID for testing
    private const string InterstitialAdUnitId = "ca-app-pub-2462813848027079/1469432018";
    private bool _isInterstitialAdReady = false;
    
    public bool IsInterstitialAdReady => _isInterstitialAdReady;

    public async Task LoadInterstitialAdAsync()
    {
        try
        {
            // Load the interstitial ad using the plugin
            await Task.Run(() => 
            {
                CrossMauiMTAdmob.Current.LoadInterstitial(InterstitialAdUnitId);
            });
            _isInterstitialAdReady = true;
        }
        catch (Exception ex)
        {
            // Log the error in production - for now just reset the flag
            _isInterstitialAdReady = false;
            System.Diagnostics.Debug.WriteLine($"Failed to load interstitial ad: {ex.Message}");
        }
    }

    public async Task ShowInterstitialAdAsync()
    {
        try
        {
            if (_isInterstitialAdReady)
            {
                await Task.Run(() => 
                {
                    CrossMauiMTAdmob.Current.ShowInterstitial();
                });
                _isInterstitialAdReady = false; // Reset flag after showing
                
                // Pre-load the next ad
                _ = Task.Run(LoadInterstitialAdAsync);
            }
        }
        catch (Exception ex)
        {
            _isInterstitialAdReady = false;
            System.Diagnostics.Debug.WriteLine($"Failed to show interstitial ad: {ex.Message}");
        }
    }
}
