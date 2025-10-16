using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Order
    {
        public int ID { get; set; }
        
        public DateTime OrderDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public string Status { get; set; } = string.Empty; //Pending(waiting for payment), OnGoing, Canceled, Delivared
        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public virtual IEnumerable<OrderProduct>? OrderProducts { get; set; }
    }
}
