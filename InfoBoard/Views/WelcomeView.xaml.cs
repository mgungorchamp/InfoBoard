using InfoBoard.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace InfoBoard.Views;

public partial class WelcomeView : ContentPage
{
    private readonly ILogger _logger;

    private readonly IHttpClientFactory _httpClientFactory;
    //ImageViewModel imageViewModel = new ImageViewModel();
    public WelcomeView(IHttpClientFactory httpClientFactory)
    {
        InitializeComponent();

        Utilities._httpClientFactory = httpClientFactory;

        _logger = Utilities.Logger(nameof(WelcomeView));
        _logger.LogInformation($"{nameof(WelcomeView)} # Constructor Called");

        /* Unmerged change from project 'InfoBoard (net8.0-ios)'
        Before:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");        

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        After:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        */

        /* Unmerged change from project 'InfoBoard (net8.0-windows10.0.19041.0)'
        Before:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");        

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        After:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        */

        /* Unmerged change from project 'InfoBoard (net8.0-android)'
        Before:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");        

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        After:
                Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");

                // Set the KeepScreenOn property to true to prevent the screen from turning off        
        */
        Debug.WriteLine($"{nameof(WelcomeView)} # Constructor Called");

        // Set the KeepScreenOn property to true to prevent the screen from turning off        
        //DeviceDisplay.Current.KeepScreenOn = true;
        //MediaManager manager = MediaManager.Instance;
        //manager.SetNavigation(Navigation);
        //_ = manager.GoTime();        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Set the KeepScreenOn property to true to prevent the screen from turning off        
            //DeviceDisplay.Current.KeepScreenOn = true;

            //var toast = Toast.Make($"Appearing! ImageDisplay");
            //await toast.Show();           
            _logger.LogInformation($"Welcome OnAppearing\n");
            Debug.WriteLine($"=> Welcome OnAppearing");

            //await Task.Delay(TimeSpan.FromSeconds(4));

            //MediaManager manager = MediaManager.Instance;
            //manager.SetNavigation(Navigation);
            //_ = manager.GoTime();


            //DeviceDisplay.Current.KeepScreenOn = true;

            //Utilities.maximumDisplayWidth = (int) (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);

            //var toast = Toast.Make($"Appearing! ImageDisplay");
            //await toast.Show();
            //mainPageImage.Source = ImageSource.FromFile(_imageViewModel.ImageSource);
            // await imageViewModel.GoTimeNow();


        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            _logger.LogError($"WELCOME #275 Exception: {ex.Message}\n");
        }

    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Set the KeepScreenOn property to false if user put our application behind
        //DeviceDisplay.Current.KeepScreenOn = false;

        //imageViewModel.StopTimersNow();

        //var toast = Toast.Make("Disappearing! ImageDisplay");
        //await toast.Show();
    }

}