using InfoBoard.Models;

namespace InfoBoard.Views.MediaViews;

public partial class WebViewViewer : ContentPage, IQueryAttributable
{
	public WebViewViewer()
	{
		InitializeComponent();
	}
    void IQueryAttributable.ApplyQueryAttributes(IDictionary<string, object> message)
    {
        var infoMessage = message["WebMedia"] as Media;
        BindingContext = infoMessage;      
    }
}