using InfoBoard.Services;
using InfoBoard.Views;
using InfoBoard.Views.MediaViews;
using System.Diagnostics;
using System.Reflection;

namespace InfoBoard;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        //NavigationPage.SetHasNavigationBar(this, false);
        //NavigationPage.SetHasBackButton(this, false);        
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

        //Routing.RegisterRoute(nameof(WelcomeView), typeof(WelcomeView));

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
    //protected override bool OnBackButtonPressed()
    //{
    //    Debug.WriteLine($"AppShell OnBackButtonPressed");
    //    return true;
    //}

    //https://github.com/dotnet/maui/issues/9300
    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        try
        {
            Debug.WriteLine($"AppShell OnNavigated {CurrentItem.ToString()}");
            string location = args.Current?.Location?.ToString();
            Debug.WriteLine($"AppShell location {location}");
            base.OnNavigated(args);
        }
       
        catch (System.Runtime.InteropServices.COMException ex)
        {
            //_logger.LogError($"\n\t #036 Exception Error Code: {(uint)ce.ErrorCode}\n" +
            //       $"Path: {media.path}\n" +
            //       $"s3key: {media.s3key}\n");

            Debug.WriteLine($"AppShell #354 OnNavigated Exception {ex.Message}");
        }
        catch (System.UriFormatException ex)
        {
        //    _logger.LogError($"\n\t #044 Exception: {exFormat.Message}\n" +
        //           $"Path: {media.path}\n");
            Debug.WriteLine($"AppShell #934 OnNavigated Exception {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"AppShell #987 OnNavigated Exception {ex.Message}");
        }
        //catch (Exception ex)
        //{
        //    Debug.WriteLine($"#879 Exception: {ex.Message}");
        //    _logger.LogError($"\n\t #879 Exception: {ex.Message}\n" +
        //        $"Path: {media.path}\n" +
        //        $"s3key: {media.s3key}\n");
        //    await DoDelay(media.timing);
        //}
    }

    //https://gist.github.com/mattjohnsonpint/7b385b7a2da7059c4a16562bc5ddb3b7
}
