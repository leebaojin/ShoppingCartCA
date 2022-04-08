using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Cart
    {
        public Cart()
        {
            Id = new Guid();
            CartDetails = new List<CartDetail>();
        }
        public Guid Id { get; set; }

        public virtual Account Account { get; set; }

        public virtual Session Session { get; set; }

        public virtual ICollection<CartDetail> CartDetails { get; set; }



    }
}
