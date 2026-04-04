using System;
using Arpg.Client;
using Arpg.Client.Extensions;
using Arpg.Tests.Mocks;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Client.Desktop.Mock;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Custom Bootstrap for Mocking
        App.Services = new ServiceCollection()
            .AddClientLogging()
            .AddClientValidators()
            .AddClientViewModels()
            .AddMockServices() // Injeta os Mocks em vez dos serviços reais
            .BuildServiceProvider();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
