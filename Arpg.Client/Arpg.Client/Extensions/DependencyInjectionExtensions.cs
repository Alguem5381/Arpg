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
    extension(IServiceCollection servicesCollection)
    {
        public IServiceCollection AddClientConfiguration()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.BrowserConsole()
                .CreateLogger();

            servicesCollection.AddLogging(logging => logging.AddSerilog());

            servicesCollection.AddValidatorsFromAssemblyContaining<CreateDtoValidator>();

            servicesCollection.AddSingleton<MainViewModel>();
            servicesCollection.AddSingleton<LoginViewModel>();
            servicesCollection.AddSingleton<RegisterViewModel>();

            servicesCollection.AddSingleton<IUserSession, UserSession>();
            servicesCollection.AddScoped<IAuthServices, AuthServices>();
            servicesCollection.AddSingleton<INavigationServices, NavigationService>();

            servicesCollection.AddTransient<AuthHeaderHandler>();

            servicesCollection.AddHttpClient<IAuthServices, AuthServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            
            return servicesCollection;

            static void Options(HttpClient client)
                => client.BaseAddress = new Uri("http://localhost:5067/");
        }
    }
}