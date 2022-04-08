using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Customer
    {
        public Customer()
        {
            Id = new Guid();
            CartDetails = new List<CartDetail>();
        }
        public Guid Id { get; set; }

        public virtual CustomerDetail CustomerDetails { get; set; }

        public virtual Session Session { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
}
