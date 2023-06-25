namespace InfoBoard.Views;

public partial class WebSiteView : ContentPage
{
	public WebSiteView()
	{
		InitializeComponent();

        

        WebView webView = new WebView
        {
            Source = new UrlWebViewSource
            {
                Url = "https://blog.xamarin.com/",
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