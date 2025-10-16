using System.ComponentModel.DataAnnotations;

namespace EBookStore.Models.CreateModel
{
    public class LoginUserModel
    {
        [Required(ErrorMessage = "Required!!!")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required!!!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
