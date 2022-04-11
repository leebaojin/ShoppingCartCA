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

        public static bool AddToCart(Customer customer, string productId, int Qty, DBContext dbContext)
        {
            Guid guidProductId;
            try
            {
                guidProductId = Guid.Parse(productId);
            }
            catch (Exception e)
            {
                return false;
            }
            if (Qty <= 0)
            {
                return false;
            }
            CartDetail cartDetail = customer.CartDetails.FirstOrDefault(x => x.Product.Id == guidProductId);

            if (cartDetail == null)
            {
                Product newProduct = dbContext.Products.FirstOrDefault(x => x.Id == guidProductId);
                if (newProduct == null)
                {
                    return false;
                }
                //To create new cartdetail and add product in.
                customer.CartDetails.Add(new CartDetail()
                {
                    Product = newProduct,
                    Quantity = Qty
                });
            }
            else
            {
                cartDetail.Quantity += Qty;
            }
            dbContext.SaveChanges();
            return true;
        }


    }
}
