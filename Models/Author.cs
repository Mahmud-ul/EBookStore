using System.ComponentModel.DataAnnotations;

namespace EBookStore.Models
{
    public class Author
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "This is Required!")]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }

        public virtual IEnumerable<Product>? Products { get; set; }
    }
}
