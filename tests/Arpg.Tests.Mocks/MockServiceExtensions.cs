using Microsoft.Extensions.DependencyInjection;
using Arpg.Client.Abstractions;

namespace Arpg.Tests.Mocks;

public static class MockServiceExtensions
{
    public static IServiceCollection AddMockServices(this IServiceCollection services)
    {
        services.AddSingleton<IUserSession, MockUserSession>();
        services.AddScoped<IAuthServices, MockAuthServices>();
        return services;
    }
}
