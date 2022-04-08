using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public virtual Guid CartId { get; set; }
    }
}
