using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models
{
    public class Payment
    {
        public int ID { get; set; }
        public DateTime PaymentDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "This is required!")]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public virtual Order? Order { get; set; }
        public int DelivaryCharge { get; set; }
        public int TotalAmount { get; set; }
        public int? Extra { get; set; }
    }
}
