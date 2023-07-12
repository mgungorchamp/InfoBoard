using Android.App;
using Android.Content.PM;
using Android.OS;

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
}
