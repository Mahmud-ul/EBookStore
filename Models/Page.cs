using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Page
    {
        public int ID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This is required!")]
        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))]
        public virtual Product? Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid page number!")]
        public int PageNumber { get; set; }
        public string? Image { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}
