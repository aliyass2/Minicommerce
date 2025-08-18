using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Minicommerce.Domain.Entities.User;

public class ApplicationUser : IdentityUser
{
    [Required, MaxLength(50)]
    public string FullName { get; set; } = string.Empty;
        
    [MaxLength(100)]
    public string? Position { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Constructor
    public ApplicationUser()
    {
    }
    
    public ApplicationUser(string userName, string fullName)
    {
        UserName = userName;
        FullName = fullName;
    }
}