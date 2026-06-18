using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Order
    {
        public int ID { get; set; }
        
        public DateTime OrderDate { get; set; }
        public DateTime? StatusDate { get; set; }
        public string Status { get; set; } = string.Empty; //Pending(waiting for payment), OnGoing, Canceled, Delivered
        public int TotalAmount { get; set; }
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"^\+?[0-9]{11,13}$", ErrorMessage = "Enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        public string Address { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public int DeliveryCharge { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User? User { get; set; }

        public virtual IEnumerable<OrderProduct>? OrderProducts { get; set; }
    }
}
