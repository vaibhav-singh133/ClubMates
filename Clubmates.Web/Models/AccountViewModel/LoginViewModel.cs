using System.ComponentModel.DataAnnotations;

namespace Clubmates.Web.Models.AccountViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is Mandatory")]
        public string Password { get; set; } = string.Empty;
    }
}
