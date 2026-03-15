using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Arpg.Client.Extensions;
using Avalonia.Markup.Xaml;
using Arpg.Client.ViewModels;
using Arpg.Client.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Client;

public class App : Application
{
    public static IServiceProvider? Services { get; set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Services ??= new ServiceCollection().AddClientConfiguration().BuildServiceProvider();
        
        var mainViewModel = Services.GetRequiredService<MainViewModel>();

        DisableAvaloniaDataAnnotationValidation();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel
                };
                break;
        }
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
            BindingPlugins.DataValidators.Remove(plugin);
    }
}