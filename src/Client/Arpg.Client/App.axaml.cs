using System;
using System.Linq;
using Arpg.Client.Extensions;
using Arpg.Client.ViewModels;
using Arpg.Client.Views;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Client;

public partial class App : Application
{
    public static IServiceProvider? Services { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services ??= new ServiceCollection().AddClientConfiguration().BuildServiceProvider();

        var rootViewModel = Services.GetRequiredService<RootViewModel>();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow
                {
                    DataContext = rootViewModel
                };
                break;

            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new BaseView
                {
                    DataContext = rootViewModel
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}