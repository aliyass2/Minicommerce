using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Infrastructure.Data.Interceptors;

namespace Minicommerce.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly DomainEventInterceptor _domainEventInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, DomainEventInterceptor domainEventInterceptor)
        : base(options)
    {
        _domainEventInterceptor = domainEventInterceptor;

    }

    // Users are already available through Identity: Users, Roles, UserRoles, etc.

    // public DbSet<Governorate> Governorates => Set<Governorate>();
    // public DbSet<District> Districts => Set<District>();
    // public DbSet<SubDistrict> SubDistricts => Set<SubDistrict>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Call base for Identity tables
        base.OnModelCreating(builder);

        // Apply all configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Configure Identity table names (optional - cleaner naming)
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

    }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_domainEventInterceptor);
    }
}