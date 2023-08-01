using InfoBoard.Services;
using InfoBoard.ViewModel;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InfoBoard.Views;

public partial class WelcomeView : ContentPage
{
    private readonly ILogger _logger;
    public WelcomeView()
	{
		InitializeComponent();

        _logger = Utilities.Logger(nameof(WelcomeView));
        _logger.LogInformation($"{nameof(WelcomeView)} # Constructor Called");
        Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");

        // Set the KeepScreenOn property to true to prevent the screen from turning off        
        DeviceDisplay.Current.KeepScreenOn = true;
        MediaManager manager = MediaManager.Instance;
        _ = manager.GoTime();
    }

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            // Set the KeepScreenOn property to true to prevent the screen from turning off        
            //DeviceDisplay.Current.KeepScreenOn = true;

            //var toast = Toast.Make($"Appearing! ImageDisplay");
            //await toast.Show();

            _logger.LogInformation($"Welcome OnAppearing\n");
            Debug.WriteLine($"=> Welcome OnAppearing");

            //await Task.Delay(TimeSpan.FromSeconds(4));

            //MediaManager manager = MediaManager.Instance;
            //await manager.GoTime();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            _logger.LogError($"WELCOME #275 Exception: {ex.Message}\n");
        }   
       
    }
}