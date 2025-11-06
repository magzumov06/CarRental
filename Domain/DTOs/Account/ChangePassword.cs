using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Account;

public class ChangePassword
{
    public int UserId { get; set; }
    [DataType(DataType.Password)]
    public required string OldPassword { get; set; }
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }
    [Compare("NewPassword")]
    public required string NewPasswordConfirm { get; set; }
}