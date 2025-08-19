using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minicommerce.Domain.Orders;
using OrderAggregate = Minicommerce.Domain.Orders.Order;

namespace Minicommerce.Infrastructure.Configurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<OrderAggregate>
{
    public void Configure(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.UserId).HasMaxLength(450).IsRequired();
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2).IsRequired();
        builder.Property(o => o.Status).HasConversion<int>().IsRequired();

        // ✅ Map by navigation and use backing field
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Owned Payment
        builder.OwnsOne(o => o.Payment, payment =>
        {
            payment.Property(p => p.PaymentMethod).HasMaxLength(50).IsRequired();
            payment.Property(p => p.TransactionId).HasMaxLength(100).IsRequired();
            payment.Property(p => p.IsMocked).HasDefaultValue(true);
        });

        builder.Ignore(o => o.DomainEvents);

        // Audit
        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.CreatedById);
        builder.Property(o => o.UpdatedById);
        builder.Property(o => o.IsActive).IsRequired().HasDefaultValue(true);
    }
}
