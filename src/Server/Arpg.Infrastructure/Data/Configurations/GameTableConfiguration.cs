using Arpg.Core.Models;
using Arpg.Core.Models.Tabletop;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class GameTableConfiguration : IEntityTypeConfiguration<GameTable>
{
    public void Configure(EntityTypeBuilder<GameTable> builder)
    {
        builder.HasKey(t => t.Id);
    }
}
