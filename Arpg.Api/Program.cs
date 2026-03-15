using Arpg.Api.Extensions;
using Arpg.Application.Extensions;
using Arpg.Infrastructure.Extensions;
using Serilog;

namespace Arpg.Api;

public static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.ApiConfigurations().Services
            .ApplicationConfigurations()
            .InfrastructureConfigurations(builder.Configuration);

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        app.UseExceptionHandler();
        
        app.UseHttpsRedirection();

        app.UseCors("AllowWasmClient");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.Run();
    }
}