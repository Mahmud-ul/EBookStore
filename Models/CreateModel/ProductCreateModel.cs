using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models.CreateModel
{
    public class ProductCreateModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Please Insert Product Name!")]
        public string Name { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }

        [Required(ErrorMessage = "Please Select Sub-Category!")]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public virtual Category? Category { get; set; }

        [Required(ErrorMessage = "This is Required!")]
        public int AuthorID { get; set; }
        [ForeignKey("AuthorID")]
        public virtual Author? Author { get; set; }

        [Required(ErrorMessage = "This is Required!")]
        public int PublisherID { get; set; }
        [ForeignKey("PublisherID")]
        public virtual Publisher? Publisher { get; set; }

        [Required(ErrorMessage = "This is Required!")]
        public int CoverID { get; set; }
        [ForeignKey("CoverID")]
        public virtual Cover? Cover { get; set; }

        public int Price { get; set; }
        public int? Discount { get; set; }
        public int? PageQuantity { get; set; }
        public string Topic { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; } = string.Empty;

        public bool Featured { get; set; }
        public bool Popular { get; set; }
        public bool New { get; set; }
        public bool PreOrderable { get; set; }
        public bool InStock { get; set; }
        public bool BestSeller { get; set; }
        public bool SlideShow { get; set; }
        public bool Status { get; set; }

        public virtual IEnumerable<Page>? Pages { get; set; }
        public virtual IEnumerable<Cart>? Carts { get; set; }
        public virtual IEnumerable<OrderProduct>? OrderProducts { get; set; }
    }
}
