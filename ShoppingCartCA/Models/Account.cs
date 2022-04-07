using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class Account
    {
        public Account()
        {
            Id = new Guid();
        }
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        public byte[] PassHash { get; set; }

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; }

        public virtual Guid CustomerId { get; set; }
    }
}
