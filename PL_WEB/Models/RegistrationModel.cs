using System.ComponentModel.DataAnnotations;
namespace PL_WEB.Models
{
    public class RegistrationModel
    {
        [Required]
        [Display(Name = "Login")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
        public string ReturnUrl { get; set; }
    }
}
