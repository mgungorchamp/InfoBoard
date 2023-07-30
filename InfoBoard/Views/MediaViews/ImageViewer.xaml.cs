using InfoBoard.Models;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Views.MediaViews;

[QueryProperty(nameof(MyMedia), "MyMedia")]
public partial class ImageViewer : ContentPage //, IQueryAttributable
{
    private readonly ILogger _logger;
    private MiniMedia contextMedia;
    public ImageViewer()
	{
		InitializeComponent();
        _logger = Utilities.Logger(nameof(ImageViewer));
        _logger.LogInformation($"{nameof(ImageViewer)} @ Constructor Called");
        //BindingContext = contextMedia;
    }

    public MiniMedia MyMedia {
        set {
            contextMedia = value;
        }
    }

    //https://stackoverflow.com/questions/72704895/net-maui-shell-navigation-is-it-possible-to-pass-a-query-parameter-and-auto-p
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        noInternetImage.IsVisible = !Utilities.isInternetAvailable();
        mainPageImage.Source = contextMedia.path;
        _logger.LogInformation($"Image view OnNavigatedTo, Name: {contextMedia.name}");
        imageName.Text = contextMedia.name;
        imageTiming.Text = contextMedia.timing.ToString();
        base.OnNavigatedTo(args);
    }



    //void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    //{
    //    var infoMessage = message["ImageMedia"] as Media;

    //    _logger.LogInformation($"Image view Displaying, Path: {infoMessage.path}");

    //    //BindingContext = infoMessage;
    //    mainPageImage.Source = infoMessage.path;
    //    _logger.LogInformation($"Image view Displaying, Name: {infoMessage.name}");
    //    imageName.Text = infoMessage.name;
    //    imageTiming.Text = infoMessage.timing.ToString();


    //    noInternetImage.IsVisible = !Utilities.isInternetAvailable();

    //}
}