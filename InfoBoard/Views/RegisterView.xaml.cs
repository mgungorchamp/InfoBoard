using InfoBoard.Services;

namespace InfoBoard.Views;

public partial class RegisterView : ContentPage
{
	public RegisterView()
	{
		InitializeComponent();
        RegisterKey.Text = "Temporary Code:" + Constants.TEMPORARY_CODE;

    }
}