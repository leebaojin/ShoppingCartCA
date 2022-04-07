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
        }
        public Guid Id { get; set; }

        //Rating of -1 means no rating given
        [Range(-1,5)]
        public int Rating { get; set; }


        [Required]
        public string Comment { get; set; }

        public virtual Account Account { get; set; }

        public virtual Guid ProductId { get; set; }


    }
}
