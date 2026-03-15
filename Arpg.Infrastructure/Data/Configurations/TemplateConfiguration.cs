using System.Text.Json;
using Arpg.Core.Models;
using Arpg.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.OwnsOne(template => template.Structure, owned =>
        {
            owned.ToJson();
            owned.OwnsMany(structures => structures.Categories);
            owned.OwnsMany(structure => structure.Fields, fieldBuilder =>
            {
                var jsonOptions = new JsonSerializerOptions();
                jsonOptions.Converters.Add(new DynamicObjectConverter());
                
                fieldBuilder.Property(f => f.DefaultValue)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, jsonOptions),
                        v => string.IsNullOrWhiteSpace(v) ? null : JsonSerializer.Deserialize<object>(v, jsonOptions)
                    );
            });
        });
    }
}