using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Category
    {
        public int ID { get; set; }

        [ForeignKey("Parent")]
        public int? MainCatID { get; set; }

        [Required(ErrorMessage = "This is Required!")]
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }

        public virtual Category? Parent { get; set; }
        public virtual IEnumerable<Product>? Products { get; set; }
    }
}
