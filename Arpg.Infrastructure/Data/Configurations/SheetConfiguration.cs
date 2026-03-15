using Arpg.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class SheetConfiguration : IEntityTypeConfiguration<Sheet>
{
    public void Configure(EntityTypeBuilder<Sheet> builder)
    {
        builder.OwnsOne(sheet => sheet.Data, owner =>
        {
            owner.ToJson();
        });
    }
}