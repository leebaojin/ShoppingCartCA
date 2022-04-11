using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class VisitorCart
    {
        public bool errFound { get; set; }

        public string cartCookieString { get; set; }

        private readonly DBContext dbContext;

        private static readonly string itemSeperator = ",";

        private static readonly string qtySeperator = ":";

        public VisitorCart(DBContext dbContext, string cartCookie = null)
        {
            if(cartCookie == null)
            {
                cartCookieString = "";
            }
            else
            {
                cartCookieString = cartCookie;
            }
            this.dbContext = dbContext;
        }

        public bool AddItem(string productId, int qty)
        {
            Product product = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(productId));

            if(product == null || qty <= 0)
            {
                return false;
            }

            int indexOfItem = cartCookieString.IndexOf(productId);

            if(indexOfItem == -1)
            {
                //cookie = productID:qty,
                cartCookieString += productId + qtySeperator + qty + itemSeperator;
            }
            else
            {
                int indexOfNextItem = cartCookieString.IndexOf(itemSeperator, indexOfItem);
                if(indexOfNextItem == -1)
                {
                    cartCookieString = "";
                    return false;
                }
                string affectedStr = cartCookieString.Substring(indexOfItem, (indexOfNextItem - indexOfItem));
                string[] affectedStrArr = affectedStr.Split(qtySeperator);
                if(affectedStrArr.Length != 2)
                {
                    cartCookieString = "";
                    return false;
                }
                try
                {
                    int itemQty = Int32.Parse(affectedStrArr[1]) + qty;

                    cartCookieString = cartCookieString.Substring(0, indexOfItem) +
                        productId + qtySeperator + itemQty + itemSeperator + cartCookieString.Substring(indexOfNextItem);
                    
                }
                catch(Exception e)
                {
                    cartCookieString = "";
                    return false;
                }

            }
            return true;
        }

        public int GetCartQuantity()
        {
            //the last array position will be empty
            return cartCookieString.Split(itemSeperator).Length - 1;
        }

        //Static method to be used for situation with only cookie string
        public static int GetCartQty(string cookieString)
        {
            return cookieString.Split(itemSeperator).Length - 1;
        }
    }
}
