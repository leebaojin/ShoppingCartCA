using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            Id = new Guid();
            ActivationCodes = new List<ActivationCode>();
        }
        public Guid Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        public double PurchasePrice { get; set; }

        public virtual Product Product { get; set; }

        public virtual ICollection<ActivationCode> ActivationCodes { get; set; }

        public virtual Guid OrderId { get; set; }

    }
}
