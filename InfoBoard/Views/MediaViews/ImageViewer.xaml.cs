using InfoBoard.Models;
using InfoBoard.Services;

namespace InfoBoard.Views.MediaViews;

public partial class ImageViewer : ContentPage, IQueryAttributable
{
	public ImageViewer()
	{
		InitializeComponent();
	}

    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    {
        var infoMessage = message["ImageMedia"] as Media;
        BindingContext = infoMessage;

        noInternetImage.IsVisible = !Utilities.isInternetAvailable();
    }
}