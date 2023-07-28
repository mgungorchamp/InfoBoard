using InfoBoard.Models;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Views.MediaViews;

public partial class ImageViewer : ContentPage, IQueryAttributable
{
    private readonly ILogger _logger;
    public ImageViewer()
	{
		InitializeComponent();
        _logger = Utilities.Logger(nameof(ImageViewer));
        _logger.LogInformation($"{nameof(ImageViewer)} @ Constructor Called");    
    }

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    {
        var infoMessage = message["ImageMedia"] as Media;
        
        _logger.LogInformation($"Image view Displaying, Path: {infoMessage.path}");

        //BindingContext = infoMessage;
        mainPageImage.Source = infoMessage.path;
        _logger.LogInformation($"Image view Displaying, Name: {infoMessage.name}");
        imageName.Text = infoMessage.name;
        imageTiming.Text = infoMessage.timing.ToString();


        noInternetImage.IsVisible = !Utilities.isInternetAvailable();
        
    }
}