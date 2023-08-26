using InfoBoard.Services;
using InfoBoard.ViewModel;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Views;

//https://www.c-sharpcorner.com/article/net-maui-qr-code-generator/
public partial class RegisterView : ContentPage
{
    private readonly ILogger _logger;
    public RegisterView()
	{
        _logger = Utilities.Logger(nameof(RegisterView));
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasBackButton(this, false);
        
        //RegisterDeviceViewModel registerDeviceViewModel = new();
        BindingContext = new RegisterDeviceViewModel();// registerDeviceViewModel; //RegisterDeviceViewModel.Instance;
        adressLabel.Text = "Activate via " + Utilities.BASE_ADDRESS;
        //qrImageButton.Source.cac
        //attemptRegisteringDevice();
        //updateQrCodeImageAndRegisterDevice();
    }

    //protected override bool OnBackButtonPressed() 
    //{
    //    Debug.WriteLine($"RegisterView OnBackButtonPressed");
    //    return true;    
    //}

    protected override void OnAppearing()
    {
        _logger.LogInformation($"\n\tRegisterView On Appearing\n");
        base.OnAppearing();
        
        ((RegisterDeviceViewModel)BindingContext).StartTimed4DeviceRegisterationEvent();
    }


    protected override void OnDisappearing()
    {
        _logger.LogInformation($"\n\tRegisterView On Disappearing\n");
        base.OnDisappearing();
        ((RegisterDeviceViewModel)BindingContext).StopTimed4DeviceRegisterationEvent();
    }
}