using InfoBoard.ViewModel;

namespace InfoBoard.Views;

//https://www.c-sharpcorner.com/article/net-maui-qr-code-generator/
public partial class RegisterView : ContentPage
{
	public RegisterView(ImageViewModel imageViewModel)
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasBackButton(this, false);
        
        RegisterDeviceViewModel registerDeviceViewModel = new();
        BindingContext = registerDeviceViewModel; //RegisterDeviceViewModel.Instance;
        registerDeviceViewModel.StartTimed4DeviceRegisterationEvent(imageViewModel);

        //attemptRegisteringDevice();
        //updateQrCodeImageAndRegisterDevice();
    }
}