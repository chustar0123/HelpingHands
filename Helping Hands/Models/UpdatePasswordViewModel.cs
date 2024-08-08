using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models
{
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Current password field cannot be left empty.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password field cannot be left empty.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password field cannot be left empty.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmationPassword { get; set; }
    }
}
