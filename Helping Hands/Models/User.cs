using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models;

public partial class User
{
    public int UserId { get; set; }
    [Required(ErrorMessage = "Username field cannot be left empty.")]
    [DisplayName("Username")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password field cannot be left empty.")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Email Address field cannot be left empty.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [DisplayName("Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Contact Number field cannot be left empty.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits.")]
    [DisplayName("Contact Number")]
    [Phone]
    public string PhoneNumber { get; set; }

    public string UserType { get; set; }

    public string Status { get; set; }


    public DateTime? DateAssigned { get; set; }

    public virtual Nurse Nurse { get; set; }

    public virtual Patient Patient { get; set; }
}
