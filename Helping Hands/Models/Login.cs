using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helping_Hands.Models
{
    public class Login
    {

        [Required(ErrorMessage = "Username field cannot be left empty.")]
        [DisplayName("Username")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Password field cannot be left empty.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
