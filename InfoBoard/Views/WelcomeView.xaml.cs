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

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        // Set the KeepScreenOn property to true to prevent the screen from turning off
        //DeviceDisplay.Current.KeepScreenOn = true;
        DeviceDisplay.Current.KeepScreenOn = true;

        //var toast = Toast.Make($"Appearing! ImageDisplay");
        //await toast.Show();
        
        _logger.LogInformation($"Welcome OnAppearing\n");

        MediaManager manager = MediaManager.Instance;
        await manager.GoTimeNow();
    }
}