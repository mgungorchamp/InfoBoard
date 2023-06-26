namespace InfoBoard.Views;

//Ref: https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.webview?view=net-maui-7.0
public partial class WebSiteView : ContentPage
{
	public WebSiteView()
	{
		InitializeComponent();

        WebView webView = new WebView
        {
            Source = new UrlWebViewSource
            {
                Url = "https://www.google.com",
            },
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        // Accomodate iPhone status bar.
        // this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

        // Build the page.
        this.Content = new StackLayout
        {
            Children =
            {

                webView
            }
        };
    }
}