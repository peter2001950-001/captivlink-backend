using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Captivlink.Api.Identity.Account
{
    public class RegisterInputModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(100)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }

        public string RedirectUrl { get; set; }
    }
}
