using Android.App;
using Android.Content.PM;
using Avalonia.Android;
using Arpg.Client;

namespace Arpg.Client.Android;

[Activity(
    Label = "Arpg.Client.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
