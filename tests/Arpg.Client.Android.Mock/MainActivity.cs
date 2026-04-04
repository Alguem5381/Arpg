using Android.App;
using Android.Content.PM;
using Android.Views;
using Arpg.Client.Extensions;
using Arpg.Tests.Mocks;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Client.Android.Mock;

[Activity(
    Label = "Arpg Mock",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : AvaloniaMainActivity
{
}