using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models.CreateModel
{
    public class PageCreateModel
    {
        public int ID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This is required!")]
        public int ProductID { get; set; }
        [ForeignKey(nameof(ProductID))]
        public virtual Product? Product { get; set; }
        public int PageNumber { get; set; }
        public IFormFile? Image { get; set; }
        public bool Status { get; set; }
    }
}
