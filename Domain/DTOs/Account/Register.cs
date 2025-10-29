using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Account;

public class Register
{
    [Required]
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}