using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class OrderProduct
    {
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public virtual Product? Product { get; set; }

        public int Quantity { get; set; }
        public int Price { get; set; }
    }
}
