using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class DataCartUpdate
    {
        public string CartItemId { get; set; }

        public int Newqty { get; set; }

        public string UpdateRow { get; set; }
    }
}
