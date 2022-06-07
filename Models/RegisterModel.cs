using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="Username is required")]
        public string? Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage ="Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
