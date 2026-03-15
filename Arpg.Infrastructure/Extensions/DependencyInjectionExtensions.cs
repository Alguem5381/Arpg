using Arpg.Application.Abstractions;
using Arpg.Application.Auth;
using Arpg.Application.Queries;
using Arpg.Application.Repositories;
using Arpg.Infrastructure.Auth;
using Arpg.Infrastructure.Configurations;
using Arpg.Infrastructure.Data;
using Arpg.Infrastructure.Queries;
using Arpg.Infrastructure.Repositories;
using Arpg.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection InfrastructureConfigurations(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            collection.AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(
                    connectionString, 
                    b => b.MigrationsAssembly("Arpg.Infrastructure")
            ));
            
            collection.Configure<EmailSettings>(
                configuration.GetSection("EmailSettings"));
            
            collection.AddScoped<ITokenServices, TokenServices>();
            collection.AddTransient<IEmailServices, EmailServices>();
            
            collection.AddScoped<IUnitOfWork, UnitOfWork>();
            
            collection.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            collection.AddScoped<IUserRepository, UserRepository>();
            collection.AddScoped<ISheetRepository, SheetRepository>();
            collection.AddScoped<ITemplateRepository, TemplateRepository>();
            collection.AddScoped<IAccountRepository, AccountRepository>();
            collection.AddScoped<ICodeRepository, CodeRepository>();
            
            collection.AddScoped<IUserQueries, UserQueries>();
            collection.AddScoped<ISheetQueries, SheetQueries>();
            collection.AddScoped<ITemplateQueries, TemplateQueries>();
            
            return collection;
        }
    }
}