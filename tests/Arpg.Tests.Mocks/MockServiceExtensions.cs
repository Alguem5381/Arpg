using Microsoft.Extensions.DependencyInjection;
using Arpg.Client.Abstractions;

namespace Arpg.Tests.Mocks;

public static class MockServiceExtensions
{
    public static IServiceCollection AddMockServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserSession, MockUserSession>();
        services.AddScoped<IAuthServices, MockAuthServices>();
        services.AddScoped<ITableServices, MockTableServices>();
        services.AddScoped<ITemplateServices, MockTemplateServices>();
        services.AddScoped<ISheetServices, MockSheetServices>();
        return services;
    }
}
