using InfoBoard.Models;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Views.MediaViews;

//[QueryProperty(nameof(MyMedia), "MyMedia")]
public partial class WebViewViewer : ContentPage//, IQueryAttributable
{
    private readonly ILogger _logger;
    //private MiniMedia contextMedia;
    public static Media contextMedia;
    public WebViewViewer()
	{
		InitializeComponent();
        _logger = Utilities.Logger(nameof(WebViewViewer));
        _logger.LogInformation($"{nameof(WebViewViewer)} # Constructor Called");
        
    }

    //public MiniMedia MyMedia {
    //    set {
    //        contextMedia = value;
    //    }
    //}
    //https://learn.microsoft.com/en-us/answers/questions/1164621/shell-navigation-and-passing-parameter-values
    //https://stackoverflow.com/questions/72704895/net-maui-shell-navigation-is-it-possible-to-pass-a-query-parameter-and-auto-p
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        WebView webView = new WebView();
        webView.Source = contextMedia.path;
        webView.WidthRequest = contextMedia.display_width;
        this.Content = webView;
        // = webView;
        _logger.LogInformation($"Web view OnNavigatedTo, Name: {contextMedia.name}");
        imageName.Text = contextMedia.name;
        imageTiming.Text = contextMedia.timing.ToString();
        
        base.OnNavigatedTo(args);
    }

    protected override void OnDisappearing() 
    {
        this.Content = null;
        base.OnDisappearing();
        _logger.LogInformation($"Web view OnDisappearing, Name: {contextMedia.name}");
    }





    //void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    //{
    //    var infoMessage = message["WebMedia"] as Media;

    //    _logger.LogInformation($"WebMedia Displaying, Path: {infoMessage.path}");

    //    //BindingContext = infoMessage;
    //    webView.Source = infoMessage.path;
    //    _logger.LogInformation($"Web view Displaying, Name: {infoMessage.name}");
    //    imageName.Text = infoMessage.name;
    //    imageTiming.Text = infoMessage.timing.ToString();
    //}
}