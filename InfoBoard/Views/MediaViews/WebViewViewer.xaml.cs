using InfoBoard.Models;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;

namespace InfoBoard.Views.MediaViews;

public partial class WebViewViewer : ContentPage, IQueryAttributable
{
    private readonly ILogger _logger;
    public WebViewViewer()
	{
		InitializeComponent();
        _logger = Utilities.Logger(nameof(WebViewViewer));
    }
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    {
        var infoMessage = message["WebMedia"] as Media;
        
        _logger.LogError($"WebMedia: {message.Keys.ToString()} \t {message.Values.ToString()}");
        
        BindingContext = infoMessage;      
    }
}