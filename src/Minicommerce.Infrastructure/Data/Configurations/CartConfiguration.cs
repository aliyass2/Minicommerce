using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minicommerce.Domain.Cart;
using CartAggregate = Minicommerce.Domain.Cart.Cart;

namespace Minicommerce.Infrastructure.Configurations.Cart;

public class CartConfiguration : IEntityTypeConfiguration<CartAggregate>
{
    public void Configure(EntityTypeBuilder<CartAggregate> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .HasMaxLength(450)
            .IsRequired();

        // ✅ Map by navigation, not by field name
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Tell EF to use the backing field "_items" for access
        builder.Navigation(c => c.Items)
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Aggregate root has DomainEvents
        builder.Ignore(c => c.DomainEvents);

        // Audit
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt);
        builder.Property(c => c.CreatedById);
        builder.Property(c => c.UpdatedById);
        builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
    }
}
