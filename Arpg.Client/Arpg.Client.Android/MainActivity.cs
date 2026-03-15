using Microsoft.Extensions.DependencyInjection;
using Arpg.Client.Extensions;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;

namespace Arpg.Client.Android;

[Activity(
    Label = "Arpg.Client.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.Services = new ServiceCollection().AddClientConfiguration().BuildServiceProvider();
        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}