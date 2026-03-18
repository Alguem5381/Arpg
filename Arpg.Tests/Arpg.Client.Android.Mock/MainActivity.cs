using Microsoft.Extensions.DependencyInjection;
using Arpg.Client.Extensions;
using Arpg.Tests.Mocks;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Avalonia;
using Avalonia.Android;

namespace Arpg.Client.Android.Mock;

[Activity(
    Label = "Arpg Mock",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode,
    WindowSoftInputMode = SoftInput.AdjustResize)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.Services = new ServiceCollection()
            .AddClientLogging()
            .AddClientValidators()
            .AddClientViewModels()
            .AddMockServices()
            .BuildServiceProvider();
        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}