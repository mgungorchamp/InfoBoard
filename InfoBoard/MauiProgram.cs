using CommunityToolkit.Maui;
using InfoBoard.ViewModel;
using InfoBoard.Views;
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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddSingleton<ImageDisplay>();
        builder.Services.AddSingleton<ImageViewModel>();


//#if DEBUG
//        builder.Logging.AddDebug();
//#endif

#if DEBUG
        builder.Services.AddLogging(configure =>
        {
            configure.AddDebug();
        });
#endif

        return builder.Build();
    }
}
