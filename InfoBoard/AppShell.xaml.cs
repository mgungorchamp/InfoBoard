using InfoBoard.Services;
using InfoBoard.Views;
using InfoBoard.Views.MediaViews;
using System.Diagnostics;

namespace InfoBoard;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        //NavigationPage.SetHasNavigationBar(this, false);
        //NavigationPage.SetHasBackButton(this, false);        
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        
        Routing.RegisterRoute(nameof(ImageDisplay), typeof(ImageDisplay));
        Routing.RegisterRoute(nameof(RegisterView), typeof(RegisterView));
        Routing.RegisterRoute(nameof(InformationView), typeof(InformationView));

        //Viewers for Media
        Routing.RegisterRoute(nameof(WebViewViewer), typeof(WebViewViewer));
        Routing.RegisterRoute(nameof(ImageViewer), typeof(ImageViewer));


        Routing.RegisterRoute(nameof(WebSiteView), typeof(WebSiteView));

        //Shell.Current.CurrentItem = imageDisplayItem;

        //Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));

        //Getting files from internet 
        //TODO: This should be optimized
        //If device lost power this process should not be repeated or WIFI connection lost
        //JSON file should be saved to local drive - and start displaying local pictutes if there are with the same name
        //If there is no picture with the same name, try to download all the picture from Internet 
        //Regardless it should compare the pictures from Internet with Local one to synchronise 
        // Delete the local one if it's no longer in the JSON
        // Redownload if the pull date is older than the file update date 
        // Download the file if there is no file with that in the local folder  

        //FileDownloadService downloader = new FileDownloadService();
        //downloader.updateFiles();

        // saveFilesToLocalDirectory.fetchAndSave();
        //Task.Run(() => downloader.downloadMediaFiles()).Wait();

    }

    //To prevent the app from changing view when the back button is pressed via mouse or remote control
    protected override bool OnBackButtonPressed()
    {
        Debug.WriteLine($"AppShell OnBackButtonPressed");
        return true;
    }
}
