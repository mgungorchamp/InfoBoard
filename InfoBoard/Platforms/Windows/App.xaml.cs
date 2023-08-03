// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using InfoBoard.Services;
using Microsoft.Extensions.Logging;

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
    }


    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"**********************************  TaskSchedulerOnUnobservedTaskException! Details: {e.Exception.ToString()}");
        _logger.LogError($"**********************************  TaskSchedulerOnUnobservedTaskException! Details: {e.Exception.ToString()}");
        //throw new NotImplementedException();
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"**********************************  Unhandled Exception! Details: {e.ExceptionObject.ToString()}");
        _logger.LogError($"**********************************  Unhandled Exception! Details: {e.ExceptionObject.ToString()}");
        //throw new NotImplementedException();
    }
    private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"********************************** FirstChance EXCEPTION! Details: {e.Exception.ToString()}");
        _logger.LogError($"********************************** FirstChance EXCEPTION! Details: {e.Exception.ToString()}");
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

