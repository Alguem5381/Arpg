using System;
using System.Net.Http;
using Arpg.Client.Abstractions;
using Arpg.Client.Services;
using Arpg.Client.ViewModels;
using Arpg.Client.ViewModels.Auth;
using Arpg.Contracts.Validators.Template;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Arpg.Client.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection servicesCollection)
    {
        public IServiceCollection AddClientConfiguration()
        {
            servicesCollection.AddClientLogging();
            servicesCollection.AddClientValidators();
            servicesCollection.AddClientViewModels();
            servicesCollection.AddClientServices();

            return servicesCollection;
        }

        public IServiceCollection AddClientLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.BrowserConsole()
                .CreateLogger();

            servicesCollection.AddLogging(logging => logging.AddSerilog());
            return servicesCollection;
        }

        public IServiceCollection AddClientValidators()
        {
            servicesCollection.AddValidatorsFromAssemblyContaining<CreateDtoValidator>();
            return servicesCollection;
        }

        public IServiceCollection AddClientViewModels()
        {
            servicesCollection.AddSingleton<RootViewModel>();
            servicesCollection.AddSingleton<MainViewModel>();
            servicesCollection.AddSingleton<AuthViewModel>();
            servicesCollection.AddSingleton<INavigationServices, NavigationService>();
            return servicesCollection;
        }

        public IServiceCollection AddClientServices()
        {
            servicesCollection.AddSingleton<IUserSession, UserSession>();
            servicesCollection.AddScoped<IAuthServices, AuthServices>();
            servicesCollection.AddScoped<ISheetServices, SheetServices>();
            servicesCollection.AddScoped<ITemplateServices, TemplateServices>();
            servicesCollection.AddScoped<ITableServices, TableServices>();
            servicesCollection.AddTransient<AuthHeaderHandler>();

            servicesCollection.AddHttpClient<IAuthServices, AuthServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();

            return servicesCollection;

            void Options(HttpClient client)
                   => client.BaseAddress = new Uri("http://localhost:5067/");
        }
    }
}