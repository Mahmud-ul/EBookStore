using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Please enter a valid Email address.")]
        [Remote(action: "IsEmailAvailable", controller: "User", AdditionalFields = "ID", ErrorMessage = "This email is already taken.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is Required")]
        [RegularExpression(@"^\+?[0-9]{11,13}$", ErrorMessage = "Enter a valid phone number.")]
        [Remote(action: "IsPhoneAvailable", controller: "User", AdditionalFields = "ID", ErrorMessage = "This Phone number is already taken.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "The field must be between 5 and 20 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please Select User-Type!!!")]
        public int UserTypeID { get; set; }
        [ForeignKey("UserTypeID")]
        public virtual UserType? UserType { get; set; }
        public virtual IEnumerable<Cart>? Carts { get; set; }
    }
}
