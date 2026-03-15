using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Arpg.Client;
using Arpg.Client.Abstractions;
using Arpg.Client.Browser;
using Arpg.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;

internal sealed partial class Program
{
    private static async Task Main(string[] args)
    {
        var builder = BuildAvaloniaApp()
            .WithInterFont();

        ServiceCollection collection = new();

        collection.AddSingleton<IDeviceServices, DeviceServices>();
        collection.AddClientConfiguration();

        App.Services = collection.BuildServiceProvider();
        
        await builder.StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();
}