using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Checkout;
using Minicommerce.Domain.Orders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CartAggregate = Minicommerce.Domain.Cart.Cart;
using CheckoutAggregate = Minicommerce.Domain.Checkout.Checkout;
using OrderAggregate = Minicommerce.Domain.Orders.Order;

namespace Minicommerce.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly DomainEventInterceptor _domainEventInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        DomainEventInterceptor domainEventInterceptor)
        : base(options)
    {
        _domainEventInterceptor = domainEventInterceptor;
    }

    // 🔹 Explicit DbSets for aggregates
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<CartAggregate> Carts => Set<CartAggregate>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CheckoutAggregate> Checkouts => Set<CheckoutAggregate>();
    public DbSet<CheckoutItem> CheckoutItems => Set<CheckoutItem>();
    public DbSet<OrderAggregate> Orders => Set<OrderAggregate>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Identity tables
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
    }
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Option A: global precision for all decimals
    configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_domainEventInterceptor);
    }
}
