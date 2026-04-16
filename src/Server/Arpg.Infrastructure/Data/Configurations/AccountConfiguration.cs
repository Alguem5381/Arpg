using Arpg.Core.Models.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Arpg.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(account => account.Id);

        builder.Property(account => account.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(account => account.Email)
            .IsUnique();

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Account>(account => account.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}