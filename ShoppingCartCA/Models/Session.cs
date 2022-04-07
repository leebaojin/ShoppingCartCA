using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Session
    {
        public Session()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
