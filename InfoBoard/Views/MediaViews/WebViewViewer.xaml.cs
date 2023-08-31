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

    //#1
    protected override void OnAppearing()
    {
        base.OnAppearing();
        MediaManager manager = MediaManager.Instance;
        contextMedia = manager.currentMedia;

        webView.Source = contextMedia.path;
        webView.WidthRequest = contextMedia.display_width;

        imageName.Text = contextMedia.name;
        imageTiming.Text = contextMedia.timing.ToString();

        //await Task.Delay(TimeSpan.FromSeconds(2));

        _logger.LogInformation($"Web view OnAppearing, Name: {contextMedia.name}");

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        //webView.Cookies = null;
        //webView.Source = null;
        ////Content = null;
        ////    webView.Source = null;
        //    webView.Resources.Clear();
        //   webView.Reload();

        
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        //webView.Source = contextMedia.path;
        //webView.WidthRequest = contextMedia.display_width;

        //imageName.Text = contextMedia.name;
        //imageTiming.Text = contextMedia.timing.ToString();

        base.OnNavigatedTo(args);
        _logger.LogInformation($"Web view OnNavigatedTo, Name: {contextMedia.name}");
        webView.IsVisible = true;
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        //webView.Source = null;
        //webView.Resources.Clear();
        //webView.Reload();

        base.OnNavigatingFrom(args);
        //_logger.LogInformation($"Web view OnNavigatingFrom, Name: {contextMedia.name}");
        webView.IsVisible = false;
    }



    ////When navigation away from this page completed - last thing to happen
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        
        //webView.Resources.Clear();

        //webView.Reload(); ****

        base.OnNavigatedFrom(args);
        //_logger.LogInformation($"Web view OnNavigatedFrom, Name: {contextMedia.name}");
        //webView.Source = null;
        //webView.Reload();
        webView.IsVisible = false;
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