using InfoBoard.ViewModel;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using CommunityToolkit.Maui.Alerts;
using Sentry;

namespace InfoBoard.Views;

public partial class ImageDisplay : ContentPage
{
    

    //ImageViewModel _imageViewModel;
    private readonly ILogger _logger;
    public ImageDisplay()
    {
        _logger = Utilities.Logger(nameof(ImageDisplay));
        
        InitializeComponent();
        //NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
        NavigationPage.SetHasBackButton(this, false);

        //BindingContext = new ImageViewModel(); 
        //_imageViewModel.NavigationSet = this.Navigation;
        Debug.WriteLine("\n\n++++++++++++++ ImageDisplay Constructor\n\n");
        _logger.LogInformation("\n++++++++++++++ ImageDisplay Constructor");
    }
    ~ImageDisplay() 
    {
        Debug.WriteLine("\n\n------------- ImageDisplay Destructor\n\n");
        _logger.LogInformation("------------- ImageDisplay Destructor");
    }

    //protected override bool OnBackButtonPressed()
    //{
    //    Debug.WriteLine($"ImageDisplay OnBackButtonPressed");
    //    return true;
    //}
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        SetWebViewBehavior();
        DeviceDisplay.MainDisplayInfoChanged += DeviceDisplay_MainDisplayInfoChanged;

        // Set the KeepScreenOn property to true to prevent the screen from turning off
        //DeviceDisplay.Current.KeepScreenOn = true;
        DeviceDisplay.Current.KeepScreenOn = true;

        //Utilities.maximumDisplayWidth = (int) (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);

        //var toast = Toast.Make($"Appearing! ImageDisplay");
        //await toast.Show();

        Debug.WriteLine($"On Appearing ImageDisplay:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");
        _logger.LogInformation($"\n------------On Appearing ImageDisplay:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");

        SentrySdk.CaptureMessage("Hello Sentry : inside Image Display OnAppearing");

        //mainPageImage.Source = ImageSource.FromFile(_imageViewModel.ImageSource);
        await ((ImageViewModel)BindingContext).GoTimeNow();
    }

     
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Set the KeepScreenOn property to false if user put our application behind
        DeviceDisplay.Current.KeepScreenOn = false;

        ((ImageViewModel)BindingContext).StopTimersNow();

        //var toast = Toast.Make("Disappearing! ImageDisplay");
        //await toast.Show();

        Debug.WriteLine($"OnDisappearing ImageDisplay:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");
        _logger.LogInformation($"\nOnDisappearing ImageDisplay:\n{mainPageImage.Source} \nApp.Current.Id{App.Current.Id}\nPage ID:{this.Id}");        
    }



    //Ref: https://dev.to/vhugogarcia/responsive-flyout-in-net-maui-4ll1
    //https://responsiveviewer.org/
    private void DeviceDisplay_MainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
    {
        SetWebViewBehavior();
    }
    private void SetWebViewBehavior()
    {
        //// Get the screen points 
        //double screenWidth = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
        
        //double screenHeight = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;

        //Debug.WriteLine(screenWidth);
        //// sizes obtained from the official bootstrap CSS 
        //switch (screenWidth)
        //{
        //    case <= 960:
        //        //webView.WidthRequest = 722;
        //        //webView.HeightRequest = screenHeight;

        //        //webView.MaximumWidthRequest = screenWidth;
        //        //webView.MaximumHeightRequest = screenHeight;

        //        //webView.MinimumWidthRequest = screenWidth;
        //        //webView.MinimumHeightRequest = screenHeight;

        //        //double theHeight = webView.Height;
        //        //double theWidth = webView.Width;


        //        break;
        //    case > 960:
        //        webView.WidthRequest = screenWidth;
        //        break;
        //}
    }




}




//await mainPageImage.FadeTo(0, 500);
//await mainPageImage.FadeTo(1, 500);
//webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>()
//    .EnableZoomControls(true);
//webView.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().EnableZoomControls(true);

//webViewTest.Source = new HtmlWebViewSource
//{
//    //Html = @"<iframe src=""https://www.youtube.com/embed/os6EZ-LFa5E""; autoplay=1; clipboard-write; encrypted-media; gyroscope; allowfullscreen></iframe>"
//    Html = @"<iframe id=""video"" src=""https://www.youtube.com/embed/os6EZ-LFa5E=1?rel=0&autoplay=1"" frameborder=""0"" allowfullscreen></iframe>"
//    //Html = @"<iframe src=""https://www.youtube.com/embed/M7lc1UVf-VE?autoplay=1&controls=0""; allowfullscreen>";
//};
//

//webViewTest.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().EnableZoomControls(false);
////https://shuttle.champlain.edu/
//webViewTest.Source = new HtmlWebViewSource
//{
//    //Html = @"<iframe   width=""600px"" height=""100%"" marginheight=""0"" frameborder=""0"" border=""0"" scrolling=""auto"" sandbox=""allow-scripts allow-forms allow-same-origin allow-presentation allow-orientation-lock allow-modals allow-popups-to-escape-sandbox allow-pointer-lock"" title=""Laptop 1 - 1440x900"" src=""https://shuttle.champlain.edu/"" ></iframe>"
//    //Html = @"<iframe src=""https://www.washingtonpost.com/"" onload='javascript:(function(o){o.style.height=o.contentWindow.document.body.scrollHeight+""px"";}(this));' style=""height:100%;width:100%;border:none;overflow:hidden;""></iframe>"
//    Html = "<script type=\"application/javascript\">\r\n\r\nfunction resizeIFrameToFitContent( iFrame ) {\r\n\r\n    iFrame.width  = iFrame.contentWindow.document.body.scrollWidth;\r\n    iFrame.height = iFrame.contentWindow.document.body.scrollHeight;\r\n}\r\n\r\nwindow.addEventListener('DOMContentLoaded', function(e) {\r\n\r\n    var iFrame = document.getElementById( 'iFrame1' );\r\n    resizeIFrameToFitContent( iFrame );\r\n\r\n    // or, to resize all iframes:\r\n    var iframes = document.querySelectorAll(\"iframe\");\r\n    for( var i = 0; i < iframes.length; i++) {\r\n        resizeIFrameToFitContent( iframes[i] );\r\n    }\r\n} );\r\n\r\n</script>\r\n\r\n<iframe src=\"https://shuttle.champlain.edu/\" id=\"iFrame1\"></iframe>"
//};

//https://responsiveviewer.org/
//webView.MinimumWidthRequest = 1280;
//webView.MinimumHeightRequest = 800;

//webView.WidthRequest= 1920;
//webView.HeightRequest= 800;

//ImageViewModel _imageViewModel;
//public ImageDisplay(ImageViewModel imageViewModel)
//{
//    InitializeComponent();

//    NavigationPage.SetHasNavigationBar(this, false);

//    imageViewModel.NNavigation = this.Navigation;
//    BindingContext = imageViewModel;// new ImageViewModel(Navigation);

//    _imageViewModel = imageViewModel;

//    //mainPageImage.Source = ImageSource.FromFile(_imageViewModel.ImageSource);
//    //DisplayAlert("Downloading Files", "Fetching files and storing them to local folder", "OK");
//    //mainPageImage.Aspect = .
//    //mediaElement.Source = FileMediaSource.FromFile("C:\\Users\\mgungor\\AppData\\Local\\Packages\\1865f6a0-a2e2-499e-b742-d4d1137346cd_9zz4h110yvjzm\\LocalState\\Media\\648e0b18dddd2-cat.jpg");
//    //mediaElement.Source = FileMediaSource.FromUri("https://sec.ch9.ms/ch9/5d93/a1eab4bf-3288-4faf-81c4-294402a85d93/XamarinShow_mid.mp4");        
//}


//public async void showMustStart()
//{
//    //await _imageViewModel.GoTimeNow();

//    //mainPageImage.PropertyChanged += async (object sender, PropertyChangedEventArgs eargs) =>
//    //{

//    //    if (eargs.PropertyName == "IsLoading")
//    //    {

//    //        if (!mainPageImage.IsLoading)
//    //        {

//    //            var mauiContext = this.Handler.MauiContext;/* get ContentPage.Handler.MauiContext */;

//    //            try
//    //            {

//    //                var res = await mainPageImage.Source.GetPlatformImageAsync(mauiContext);
//    //                //mainPageImage.Source = res.Value;
//    //            }
//    //            catch (Exception e)
//    //            {

//    //                Debug.WriteLine($"EXCEPTION CAUGHT {e}");

//    //                mainPageImage.Source = "https://upload.wikimedia.org/wikipedia/commons/d/dd/Achtung.svg";
//    //            }
//    //        }
//    //    }
//    //};

//}