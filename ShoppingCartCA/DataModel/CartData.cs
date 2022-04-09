using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class CartData
    {
        public static int GetCartSize(Customer customer)
        {
            int cartsize = 0;

            foreach (CartDetail cartDetail in customer.CartDetails)
            {
                cartsize += cartDetail.Quantity;
            }
            return cartsize;
        }
    }
}
