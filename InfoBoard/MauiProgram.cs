using CommunityToolkit.Maui;
using InfoBoard.Views;
using InfoBoard.Views.MediaViews;
using Microsoft.Extensions.Logging;

namespace InfoBoard;

/*
 
 Create a .NET MAUI app
 https://learn.microsoft.com/en-us/dotnet/maui/tutorials/notes-app/?view=net-maui-7.0

 */

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement() // https://devblogs.microsoft.com/dotnet/announcing-dotnet-maui-communitytoolkit-mediaelement/

            // Add this section anywhere on the builder:
            .UseSentry(options => {
                // The DSN is the only required setting.
                options.Dsn = "https://1f8a812ef42b21a539e74b6455bb2084@o4505670907592704.ingest.sentry.io/4505670910607360";

                // Use debug mode if you want to see what the SDK is doing.
                // Debug messages are written to stdout with Console.Writeline,
                // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                // This option is not recommended when deploying your application.
                options.Debug = true;

                // Set TracesSampleRate to 1.0 to capture 100% of transactions for performance monitoring.
                // We recommend adjusting this value in production.
                options.TracesSampleRate = 1.0;                
                // Other Sentry options can be set here.
            })


            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });       

        builder.Services.AddSingleton<ImageDisplay>();
        //builder.Services.AddSingleton<WebSiteView>();
        builder.Services.AddTransient<RegisterView>();


        //builder.Services.AddSingleton<EmptyPage>();
        builder.Services.AddSingleton<ImageViewer>();
        builder.Services.AddSingleton<WebViewViewer>();
        builder.Services.AddSingleton<WelcomeView>();

        builder.Services.AddSingleton<InformationView>();

        //builder.Services.AddTransient<ImageViewModel>();


        //#if DEBUG
        //        builder.Logging.AddDebug();
        //#endif

#if DEBUG
        builder.Services.AddLogging(configure =>
        {
            configure.AddDebug();
        });
#endif
       // CustomizeWebViewHandler();

        return builder.Build();
    }
//    private static void CustomizeWebViewHandler()
//    {
//#if ANDROID26_0_OR_GREATER
//    Microsoft.Maui.Handlers.WebViewHandler.Mapper.ModifyMapping(
//        nameof(Android.Webkit.WebView.WebChromeClient),
//        (handler, view, args) => handler.PlatformView.SetWebChromeClient(new MyWebChromeClient(handler)));
//#endif
//    }

}
