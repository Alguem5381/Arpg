using Arpg.Application.Services;
using Arpg.Application.Security;
using Arpg.Application.Auth;
using Arpg.Core.Interfaces.Security;
using Arpg.Contracts.Validators.Template;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Arpg.Application.Extensions;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection ApplicationConfigurations()
        {
            collection.AddValidatorsFromAssemblyContaining<CreateDtoValidator>();

            collection.AddScoped<SheetServices>();
            collection.AddScoped<StructureServices>();
            collection.AddScoped<TemplateServices>();
            collection.AddScoped<UserServices>();
            collection.AddScoped<AccountServices>();
            collection.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

            return collection;
        }
    }
}