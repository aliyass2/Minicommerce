using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minicommerce.Domain.Cart;

namespace Minicommerce.Infrastructure.Configurations.Cart;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.ProductName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(ci => ci.Quantity)
            .IsRequired();
        // Audit fields
        builder.Property(ci => ci.CreatedAt).IsRequired();
        builder.Property(ci => ci.UpdatedAt);
        builder.Property(ci => ci.CreatedById);
        builder.Property(ci => ci.UpdatedById);
        builder.Property(ci => ci.IsActive).IsRequired().HasDefaultValue(true);
    }
}
