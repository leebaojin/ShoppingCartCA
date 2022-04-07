using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Product
    {
        public Product()
        {
            Id = new Guid();
            SimilarProducts = new List<Product>();
            ReferenceBy = new List<Product>();
            Reviews = new List<Review>();
        }
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(300)]
        public string Desc { get; set; }

        //image link of the object "~/pictures/dotnet_paypal.jpg
        [MaxLength(50)]
        [Required]
        public string Img { get; set; }

        //image link of the download file "~/document/dotnet_paypal.pdf
        [MaxLength(50)]
        [Required]
        public string DownloadFile { get; set; }

        [Required]
        public double Price { get; set; }

        //Should generate another table for this many-to-many relationship
        
        //[InverseProperty("ReferenceBy")]
        public virtual ICollection<Product> SimilarProducts { get; set; }

        //[InverseProperty("SimilarProducts")]
        public virtual ICollection<Product> ReferenceBy { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

    }
}
