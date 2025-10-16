using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Cart
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }
        public int Quantity { get; set; }
    }
}
