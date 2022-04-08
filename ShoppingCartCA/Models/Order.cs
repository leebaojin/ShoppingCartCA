using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Order
    {
        public Order()
        {
            Id = new Guid();
            OrderDetails = new List<OrderDetail>();
        }
        public Guid Id { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Guid CustomerId { get; set; }
    }
}
