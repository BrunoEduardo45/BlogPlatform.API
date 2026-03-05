using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; }

    [Compare(nameof(NewPassword))]
    public string ConfirmPassword { get; set; }
}