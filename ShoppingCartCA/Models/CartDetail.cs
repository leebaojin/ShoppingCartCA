using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class CartDetail
    {
        public CartDetail()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Product Product { get; set; }

    }
}
