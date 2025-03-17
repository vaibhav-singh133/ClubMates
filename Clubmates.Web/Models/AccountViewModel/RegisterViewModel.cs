using System.ComponentModel.DataAnnotations;

namespace Clubmates.Web.Models.AccountViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="FullName is Mandatory")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage ="Email Id is Mandatory")]
        [EmailAddress(ErrorMessage ="This is not a Valid email address, Use this format")]
        public string? Email { get; set; } = string.Empty;
        [Required(ErrorMessage ="Please Provide a password to secure your account")]
        public string? Password { get; set; } = string.Empty;
        [Required(ErrorMessage ="This Password has to match the Privious one")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}
