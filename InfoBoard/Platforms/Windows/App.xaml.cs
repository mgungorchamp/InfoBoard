// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Diagnostics;

namespace InfoBoard.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    /// 
    private readonly ILogger _logger;
    public App()
    {
        this.InitializeComponent();
        _logger = Utilities.Logger(nameof(App) + "WindowsMKG");

        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        Microsoft.UI.Xaml.Application.Current.UnhandledException += Current_UnhandledException; 
    }

    private void Current_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"**********************************  Microsoft.UI.Xaml.Application.Current.UnhandledException! Details: {e.Exception.ToString()}");
        _logger.LogError($"**********************************  Microsoft.UI.Xaml.Application.Current.UnhandledException! Details: {e.Exception.ToString()}");
        var newExc = new Exception("Microsoft.UI.Xaml.Application.Current.UnhandledException", e.Exception);
        LogUnhandledException(newExc);
    }

    //Ref: https://peterno.wordpress.com/2015/04/15/unhandled-exception-handling-in-ios-and-android-with-xamarin/
    // https://gist.github.com/mattjohnsonpint/7b385b7a2da7059c4a16562bc5ddb3b7
    // https://github.com/dotnet/maui/discussions/653

    #region Error handling
    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        
        System.Diagnostics.Debug.WriteLine($"**********************************  TaskSchedulerOnUnobservedTaskException! Details: {e.Exception.ToString()}");
        _logger.LogError($"**********************************  TaskSchedulerOnUnobservedTaskException! Details: {e.Exception.ToString()}");
        //throw new NotImplementedException();
        var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);
        LogUnhandledException(newExc);
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"**********************************  Unhandled Exception! Details: {e.ExceptionObject.ToString()}");
        _logger.LogError($"**********************************  Unhandled Exception! Details: {e.ExceptionObject.ToString()}");
        //throw new NotImplementedException();
        var newExc = new Exception("CurrentDomain_UnhandledException " + e.ExceptionObject.ToString());
        LogUnhandledException(newExc);
    }
    private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"********************************** FirstChance EXCEPTION! Details: {e.Exception.ToString()}");
        _logger.LogError($"********************************** FirstChance EXCEPTION! Details: {e.Exception.ToString()}");
        var newExc = new Exception("CurrentDomain_FirstChanceException", e.Exception);
        LogUnhandledException(newExc);
    }

    internal void LogUnhandledException(Exception exception)
    {
        try
        {
            const string errorFileName = "Fatal.log";
            var libraryPath = Path.Combine(FileSystem.CacheDirectory, "InfoBoardLogs"); ; // iOS: Environment.SpecialFolder.Resources
            var errorFilePath = Path.Combine(libraryPath, errorFileName);
            var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
            DateTime.Now, exception.ToString());
            //File.WriteAllText(errorFilePath, errorMessage);
            File.AppendAllText(errorFilePath, errorMessage);
            // Log to Android Device Logging.
            
            _logger.LogError($"**********************************  Error Logged ! Details at: {errorFilePath} Message {errorMessage}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"**********************************  LogUnhandledException Exception! Details: {ex.Message}");
        }
    }
    [Conditional("DEBUG")]
    private async void DisplayCrashReport()
    {
        const string errorFilename = "Fatal.log";
        var libraryPath = Path.Combine(FileSystem.CacheDirectory, "InfoBoardLogs"); ;
        var errorFilePath = Path.Combine(libraryPath, errorFilename);

        if (!File.Exists(errorFilePath))
        {
            return;
        }

        var errorText = File.ReadAllText(errorFilePath);

        var toast = Toast.Make($"Crash Report {errorText}", ToastDuration.Long);
        
        await toast.Show();
        
        File.Delete(errorFilePath);        
    }
#endregion

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

