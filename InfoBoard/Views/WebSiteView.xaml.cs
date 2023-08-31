using InfoBoard.Models;


namespace InfoBoard.Views;

//Ref: https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.webview?view=net-maui-7.0
//Param passing:
//https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/navigation
//https://learn.microsoft.com/en-us/answers/questions/1164621/shell-navigation-and-passing-parameter-values

[QueryProperty(nameof(MediaInformation), "MediaInformationParam")]
public partial class WebSiteView : ContentPage
{

    public WebSiteView()
    {
        InitializeComponent();
    }

    Media mediaInfo;
    public Media MediaInformation
    {
        get => mediaInfo;
        set
        {
            mediaInfo = value;
            //Url = mediaInfo.presignedURL;
            showWebSite(mediaInfo.path);
            OnPropertyChanged();
        }
    }

    private void showWebSite(string urlPath)
    {
        Url = urlPath;
        //await Task.Delay(TimeSpan.FromSeconds(10));
    }
    //string url;
    public string Url
    {
        get => webView.Source.ToString();
        set
        {
            webView.Source = value;
            webView.Reload();
            OnPropertyChanged();
        }
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        //webView.Source = "https://learn.microsoft.com/dotnet/maui";
        //Url = "https://cdn.destguides.com/files/store/itinerarystop/29947/background_image/webp_large_202112291737-0fca27aa82c1f5eb16403d66d58cb1a7.webp";
        //Url = "https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mp4-file.mp4";
        //Doesnt work Url = "https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-mov-file.mov";
        //Doesnt work Url = "https://www.learningcontainer.com/wp-content/uploads/2020/05/sample-flv-file.flv";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        webView.Source = null;
    }
}


/*
 
   //WebView webvView = new WebView
        //{
        //    Source = "https://learn.microsoft.com/dotnet/maui"
        //};

        //WebView webView = new WebView
        //{
        //    Source = new UrlWebViewSource
        //    {
        //        Url = "https://www.google.com",
        //    },
        //    //VerticalOptions = LayoutOptions.FillAndExpand
        //};

        //// Accomodate iPhone status bar.
        //// this.Padding = new Thickness(10, Device.OnPlatform(20, 0, 0), 10, 5);

        //// Build the page.
        //this.Content = new Grid
        //{
        //    Children =
        //    {

        //        webView
        //    }
        //};
 
 */