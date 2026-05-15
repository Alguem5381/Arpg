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
                .WriteTo.Console()
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
            
            servicesCollection.AddTransient<TablesSectionViewModel>();
            servicesCollection.AddTransient<TemplatesSectionViewModel>();
            servicesCollection.AddTransient<SheetsSectionViewModel>();
            
            servicesCollection.AddTransient<TablesListViewModel>();
            servicesCollection.AddTransient<TemplatesListViewModel>();
            servicesCollection.AddTransient<TemplateEditorViewModel>();
            servicesCollection.AddTransient<SheetsListViewModel>();
            servicesCollection.AddTransient<SheetEditorViewModel>();

            servicesCollection.AddSingleton<INavigationServiceFactory, NavigationServiceFactory>();
            servicesCollection.AddSingleton<INavigationServices>(sp => 
                sp.GetRequiredService<INavigationServiceFactory>().CreateLocalNavigation());
            return servicesCollection;
        }

        public IServiceCollection AddClientServices()
        {
            servicesCollection.AddSingleton<IUserSession, UserSession>();
            servicesCollection.AddTransient<AuthHeaderHandler>();

            servicesCollection.AddHttpClient<IAuthServices, AuthServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            servicesCollection.AddHttpClient<ISheetServices, SheetServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            servicesCollection.AddHttpClient<ITemplateServices, TemplateServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();
            servicesCollection.AddHttpClient<ITableServices, GameTableServices>(Options)
                .AddHttpMessageHandler<AuthHeaderHandler>();

            return servicesCollection;

            void Options(HttpClient client)
                   => client.BaseAddress = new Uri("http://localhost:5067/");
        }
    }
}
