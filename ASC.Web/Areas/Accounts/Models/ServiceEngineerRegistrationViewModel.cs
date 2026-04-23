using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Areas.Accounts.Models
{
    public class ServiceEngineerRegistrationViewModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool IsEdit { get; set; }

        public bool IsActive { get; set; }

    }
}
