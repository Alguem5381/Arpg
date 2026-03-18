using System;
using System.Net.Http;

using Arpg.Client.Abstractions;
using Arpg.Client.Services;
using Arpg.Client.ViewModels;
using Arpg.Client.ViewModels.Auth;

using Arpg.Contracts.Validators.Template;

using Microsoft.Extensions.DependencyInjection;

using FluentValidation;
using Serilog;

namespace Arpg.Client.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddClientConfiguration(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddClientLogging();
        servicesCollection.AddClientValidators();
        servicesCollection.AddClientViewModels();
        servicesCollection.AddClientServices();

        return servicesCollection;
    }

    public static IServiceCollection AddClientLogging(this IServiceCollection servicesCollection)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.BrowserConsole()
            .CreateLogger();

        servicesCollection.AddLogging(logging => logging.AddSerilog());
        return servicesCollection;
    }

    public static IServiceCollection AddClientValidators(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddValidatorsFromAssemblyContaining<CreateDtoValidator>();
        return servicesCollection;
    }

    public static IServiceCollection AddClientViewModels(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddSingleton<MainViewModel>();
        servicesCollection.AddSingleton<LoginViewModel>();
        servicesCollection.AddSingleton<ValidateViewModel>();
        servicesCollection.AddSingleton<INavigationServices, NavigationService>();
        return servicesCollection;
    }

    public static IServiceCollection AddClientServices(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddSingleton<IUserSession, UserSession>();
        servicesCollection.AddScoped<IAuthServices, AuthServices>();
        servicesCollection.AddTransient<AuthHeaderHandler>();

        servicesCollection.AddHttpClient<IAuthServices, AuthServices>(Options)
            .AddHttpMessageHandler<AuthHeaderHandler>();

        return servicesCollection;

        static void Options(HttpClient client)
            => client.BaseAddress = new Uri("http://localhost:5067/");
    }
}