﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Helping_Hands.Models.Register
{
    public class RegisterViewModel
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Username field cannot be left empty.")]
        [DisplayName("Username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password field cannot be left empty.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email Address field cannot be left empty.")]
        [DisplayName("Email Address")]
        [EmailAddress]
        //[GmailEmail(ErrorMessage = "Invalid email address. The email must contain '@gmail.com'.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact Number field cannot be left empty.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits.")]
        [DisplayName("Contact Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        public string UserType { get; set; } = "P";

        public string Status { get; set; } = "A";
        public DateTime? DateAssigned { get; set; } = DateTime.Now;
    }
    public class GmailEmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // Let RequiredAttribute handle the required validation

            // Check if the email address contains "@gmail.com"
            string email = value.ToString();
            return email.EndsWith("@gmail.com", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
