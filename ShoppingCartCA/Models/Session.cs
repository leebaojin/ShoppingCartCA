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
			Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        public Guid Id { get; set; }
		
		public long Timestamp { get; set; }

        public virtual Customer Customer { get; set; }

    }
}
