using Arpg.Application.Auth;
using Arpg.Core.Models;
using Arpg.Core.Models.Customer;
using Arpg.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class CodeConfiguration : IEntityTypeConfiguration<Code>
{
    public void Configure(EntityTypeBuilder<Code> builder)
    {
        builder.HasKey(code => code.Key);
        
        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(code => code.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}