using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASC.Web.Areas.Accounts.Models
{
    public class CustomerRegistrationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsEdit { get; set; }
        public bool IsActive { get; set; }

    }
}
