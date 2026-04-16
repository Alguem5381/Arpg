using System.Text.Json;
using Arpg.Core.Models.Customer;
using Arpg.Core.Models.Definitions;
using Arpg.Infrastructure.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(template => template.OwnerId)
            .IsRequired();

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