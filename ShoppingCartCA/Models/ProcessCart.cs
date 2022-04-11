using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class ProcessCart
    {
        public bool Success { get; set; }
        public double DetailPrice { get; set; }
        public double TotalPrice { get; set; }


        public ProcessCart(bool success, double detailprice, double totalprice)
        {
            Success = success;
            DetailPrice = detailprice;
            TotalPrice = totalprice;
        }
    }
}
