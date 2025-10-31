using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    [Required]
    public required string FullName { get; set; }
    public override required string Email { get; set; }
    public string? ProfilePicture { get; set; }
    public Roles Role { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }
    public List<Rental>? Rentals { get; set; }
}