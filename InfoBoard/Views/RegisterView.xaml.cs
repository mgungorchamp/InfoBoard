using InfoBoard.ViewModel;

namespace InfoBoard.Views;

//https://www.c-sharpcorner.com/article/net-maui-qr-code-generator/
public partial class RegisterView : ContentPage
{
	public RegisterView()
	{
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasBackButton(this, false);
        BindingContext = RegisterDeviceViewModel.Instance;
        //attemptRegisteringDevice();
        //updateQrCodeImageAndRegisterDevice();
    }
}