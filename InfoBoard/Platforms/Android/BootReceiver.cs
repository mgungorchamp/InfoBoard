using Android.App;
using Android;
using Android.Content;
using Android.Widget;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]


namespace InfoBoard.Platforms.Android
{
    [BroadcastReceiver(Name = "com.guzelboard.infoboard.BootReceiver", Exported = true, Enabled = true, DirectBootAware = true)]
    [IntentFilter(new[] {  Intent.ActionReboot,
        Intent.ActionBootCompleted
        , Intent.ActionLockedBootCompleted
        , Intent.ActionMyPackageReplaced
        , Intent.ActionUserInitialize
        , "android.intent.action.QUICKBOOT_POWERON"
        , "com.htc.intent.action.QUICKBOOT_POWERON" }, Categories = new[] { Intent.CategoryDefault })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == null || (!intent.Action.Equals(Intent.ActionBootCompleted)
                                          && !intent.Action.Equals(Intent.ActionReboot)
                                          && !intent.Action.Equals(Intent.ActionLockedBootCompleted)
                                          && !intent.Action.Equals(Intent.ActionUserInitialize)
                                          && !intent.Action.Equals("android.intent.action.QUICKBOOT_POWERON")
                                          && !intent.Action.Equals("com.htc.intent.action.QUICKBOOT_POWERON"))) return;
            Toast.MakeText(context, "GuzelBoard Starting...", ToastLength.Long)?.Show();
            var s = new Intent(context, typeof(MainActivity));
            s.AddFlags(ActivityFlags.ResetTaskIfNeeded);
            s.AddFlags(ActivityFlags.ReorderToFront);
            s.AddFlags(ActivityFlags.NewTask);
            context.StartActivity(s);
        }
    }
}


 