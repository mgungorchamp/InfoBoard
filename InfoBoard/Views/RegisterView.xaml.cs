using CommunityToolkit.Maui.Alerts;
using InfoBoard.ViewModel;
using System.Diagnostics;

namespace InfoBoard.Views;

//https://www.c-sharpcorner.com/article/net-maui-qr-code-generator/
public partial class RegisterView : ContentPage
{
	public RegisterView()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasBackButton(this, false);
        RegisterDeviceViewModel registerDeviceViewModel = new();
        BindingContext = new RegisterDeviceViewModel();// registerDeviceViewModel; //RegisterDeviceViewModel.Instance;


        //attemptRegisteringDevice();
        //updateQrCodeImageAndRegisterDevice();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((RegisterDeviceViewModel)BindingContext).StartTimed4DeviceRegisterationEvent();
        Debug.WriteLine($"RegisterView On Appearing");
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ((RegisterDeviceViewModel)BindingContext).StopTimed4DeviceRegisterationEvent();
        Debug.WriteLine($"RegisterView On Disappearing");
    }
}