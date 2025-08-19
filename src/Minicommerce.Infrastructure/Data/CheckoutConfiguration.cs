using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minicommerce.Domain.Checkout;
using CheckoutAggregate = Minicommerce.Domain.Checkout.Checkout;

namespace Minicommerce.Infrastructure.Configurations.Checkout;

public class CheckoutConfiguration : IEntityTypeConfiguration<CheckoutAggregate>
{
    public void Configure(EntityTypeBuilder<CheckoutAggregate> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId).HasMaxLength(450).IsRequired();
        builder.Property(c => c.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Status).HasConversion<int>().IsRequired();

        // ✅ Map by navigation and use backing field
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey("CheckoutId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Items)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Owned Payment
        builder.OwnsOne(c => c.Payment, payment =>
        {
            payment.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
            payment.Property(p => p.TransactionId).HasMaxLength(100).IsRequired();
            payment.Property(p => p.IsMocked).HasDefaultValue(true);
        });

        builder.Ignore(c => c.DomainEvents);

        // Audit
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.CreatedById);
        builder.Property(c => c.UpdatedById);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
    }
}
