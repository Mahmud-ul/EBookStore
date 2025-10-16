using System.ComponentModel.DataAnnotations;

namespace EBookStore.Models
{
    public class UserType
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter User-Type!!!")]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
