using System;
using Android.App;
using Android.Runtime;
using Arpg.Client.Extensions;
using Arpg.Tests.Mocks;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using Arpg.Client;

namespace Arpg.Client.Android.Mock;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    public AndroidApp(IntPtr handle, JniHandleOwnership transfer)
        : base(handle, transfer)
    {
    }

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
