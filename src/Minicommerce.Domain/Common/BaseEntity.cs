using System;

namespace Minicommerce.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedById { get; set; }  // Changed to string for Identity
    public string? UpdatedById { get; set; }   // Changed to string for Identity
    public bool IsActive { get; set; } = true;
    
    // Soft delete fields
    public DateTime? DeletedAt { get; set; }
    public string? DeletedById { get; set; }
    
    // Computed property to check if entity is deleted
    public bool IsDeleted => DeletedAt.HasValue;


    }
