using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class ReviewData
    {
        public static Review GetReview(Product product, Customer customer)
        {
            Review review = null;
            if (customer.CustomerDetails == null)
            {
                return null;
            }
            foreach (Review rw in product.Reviews.ToList())
            {
                if(rw.Customer.Id == customer.Id)
                {
                    review = rw;
                    break;
                }
            }
            return review;
        }

        public static bool CanReview(Product product, Customer customer)
        {
            //Only customers who purchased the product can review
            if(customer.CustomerDetails == null)
            {
                return false;
            }
            foreach(Order order in customer.Orders.ToList())
            {
                foreach(OrderDetail od in order.OrderDetails.ToList())
                {
                    if(od.Product.Id == product.Id)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
