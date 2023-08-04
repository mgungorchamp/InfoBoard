using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using InfoBoard.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InfoBoard;
 
[Activity(Theme = "@style/Maui.SplashTheme",
    Enabled = true,
    Exported = true,
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTask,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Platform.Intent.ActionAppAction }, Categories = new[] { global::Android.Content.Intent.CategoryLauncher })]


public class MainActivity : MauiAppCompatActivity
{
    
    // In MainActivity
    //Ref:https://peterno.wordpress.com/2015/04/15/unhandled-exception-handling-in-ios-and-android-with-xamarin/
    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);
    
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;        

        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
        

        //Xamarin.Forms.Forms.Init(this, bundle);
        DisplayCrashReport();

        //var app = new App();
        //LoadApplication(app);
    }

    private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
    {
        var newExc = new Exception("AndroidEnvironment_UnhandledExceptionRaiser", e.Exception);
        System.Diagnostics.Debug.WriteLine($"**********************************  AndroidEnvironment_UnhandledExceptionRaiser! Details: {e.Exception.ToString()}");
        

        LogUnhandledException(newExc);
    }

    #region Error handling
    private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    {
        var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
        System.Diagnostics.Debug.WriteLine($"**********************************  TaskSchedulerOnUnobservedTaskException! Details: {unobservedTaskExceptionEventArgs.Exception.ToString()}");

        LogUnhandledException(newExc);
    }
    private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", e.Exception);

        System.Diagnostics.Debug.WriteLine($"********************************** FirstChance EXCEPTION! Details: {e.Exception.ToString()}");

        LogUnhandledException(newExc);
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
        var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
        System.Diagnostics.Debug.WriteLine($"**********************************  Unhandled Exception! Details: {unhandledExceptionEventArgs.ExceptionObject.ToString()}");

        LogUnhandledException(newExc);
    }

    internal static void LogUnhandledException(Exception exception)
    {
        try
        {
            const string errorFileName = "Fatal.log";
            var libraryPath = Android.OS.Environment.DownloadCacheDirectory.Path;   // iOS: Environment.SpecialFolder.Resources
            var errorFilePath = Path.Combine(libraryPath, errorFileName);
            var errorMessage = String.Format("Time: {0}\r\nError: Unhandled Exception\r\n{1}",
            DateTime.Now, exception.ToString());
            File.WriteAllText(errorFilePath, errorMessage);

            // Log to Android Device Logging.
            Android.Util.Log.Error("Crash Report", errorMessage);
        }
        catch(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"**********************************  Unhandled Exception! {ex.Message}");
        }
    }

    //<summary>
    // If there is an unhandled exception, the exception information is diplayed 
    // on screen the next time the app is started (only in debug configuration)
    //</summary>
    [Conditional("DEBUG")]
    private void DisplayCrashReport()
    {
        const string errorFilename = "Fatal.log";
        var libraryPath = Android.OS.Environment.DownloadCacheDirectory.Path;
        var errorFilePath = Path.Combine(libraryPath, errorFilename);

        if (!File.Exists(errorFilePath))
        {
            return;
        }

        var errorText = File.ReadAllText(errorFilePath);
        new AlertDialog.Builder(this)
            .SetPositiveButton("Clear", (sender, args) =>
            {
                File.Delete(errorFilePath);
            })
            .SetNegativeButton("Close", (sender, args) =>
            {
                // User pressed Close.
            })
            .SetMessage(errorText)
            .SetTitle("Crash Report")
            .Show();
    } 
 #endregion  
}
