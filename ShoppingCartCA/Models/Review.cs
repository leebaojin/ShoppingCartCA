using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Review
    {
        public Review()
        {
            Id = new Guid();
            Rating = -1; //No rating
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        public Guid Id { get; set; }

        //Rating of -1 means no rating given
        [Range(-1,5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public long Timestamp { get; set; }
        //dateTime = dateTime.AddSeconds( unixTimeStamp ).ToLocalTime()

        public virtual Customer Customer { get; set; }

        public virtual Product Product { get; set; }


    }
}
