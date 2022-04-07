using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class ActivationCode
    {
        public ActivationCode()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }

        public virtual Guid OrderDetailId { get; set; }

        public virtual Guid ProductId { get; set; }
    }
}
