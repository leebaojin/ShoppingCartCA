using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class VisitorCart
    {
        public bool ErrFound { get; set; }

        public string cartCookieString { get; set; }

        private double totalCost;
        public double TotalCost {
            get
            {
                if(totalCost <= 0)
                {
                    this.PopulateList();
                }
                return totalCost;
            }}

        private List<CartDetail> visitorCartList;
        public List<CartDetail> VisitorCartList {
            get { 
            if(visitorCartList.Count == 0)
                {
                    this.PopulateList();
                }
                return visitorCartList;
            
            } }

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
            totalCost = 0;
            visitorCartList = new List<CartDetail>();
        }

        public bool AddItem(string productId, int qty)
        {
            if(qty <= 0)
            {
                return false;
            }

            return UpdateCartItem(productId, qty);
        }

        public bool UpdateItem(string productId, int qty)
        {
            //Note that quantity can be 0.
            //Item will be removed as a result.
            if (qty < 0)
            {
                return false;
            }

            return UpdateCartItem(productId, qty, true);
        }


        private bool UpdateCartItem(string productId, int qty, bool absQty = false)
        {
            //If absQty is true, then the quantity will replace the current quantity
            //Otherwise, it will just add the quanity to the current quantity

            Product product = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(productId));

            if (product == null)
            {
                return false;
            }

            int indexOfItem = cartCookieString.IndexOf(productId);

            if (indexOfItem == -1)
            {
                //cookie = productID:qty,
                cartCookieString += productId + qtySeperator + qty + itemSeperator;
            }
            else
            {
                int indexOfNextItem = cartCookieString.IndexOf(itemSeperator, indexOfItem);
                if (indexOfNextItem == -1)
                {
                    cartCookieString = "";
                    return false;
                }
                string affectedStr = cartCookieString.Substring(indexOfItem, (indexOfNextItem - indexOfItem));
                string[] affectedStrArr = affectedStr.Split(qtySeperator);
                if (affectedStrArr.Length != 2)
                {
                    cartCookieString = "";
                    return false;
                }
                try
                {
                    int itemQty;
                    if (absQty)
                    {
                        itemQty = qty;
                    }
                    else
                    {
                        itemQty = Int32.Parse(affectedStrArr[1]) + qty;
                    }

                    if(itemQty > 0)
                    {
                        //Modify item if > 0
                        cartCookieString = cartCookieString.Substring(0, indexOfItem) +
                                 productId + qtySeperator + itemQty + cartCookieString.Substring(indexOfNextItem);
                    }
                    else
                    {
                        //Delete item if 0
                        //The second part of the substring will start 1 index after as the "," will be ignored (removed)
                        if((indexOfNextItem + 1) == cartCookieString.Length)
                        {
                            cartCookieString = cartCookieString.Substring(0, indexOfItem);
                        }
                        else
                        {
                            cartCookieString = cartCookieString.Substring(0, indexOfItem) + cartCookieString.Substring(indexOfNextItem + 1);
                        }
                    }
                }
                catch (Exception e)
                {
                    cartCookieString = "";
                    return false;
                }

            }
            //Resetting the calculated parameters when there is a change
            visitorCartList.Clear();
            totalCost = 0;
            return true;
        }

        public bool PopulateList()
        {
            double cost = 0;
            string[] cartItems = cartCookieString.Split(itemSeperator);
            for(int i = 0; i<cartItems.Length; i++)
            {
                string[] itemDetail = cartItems[i].Split(qtySeperator);
                if(itemDetail.Length != 2)
                {
                    cartCookieString = "";
                    return false;
                }

                string productId = itemDetail[0];
                int prdQty = 0;
                    try
                    {
                        prdQty = Int32.Parse(itemDetail[1]);
                    }
                    catch(Exception e)
                    {
                        cartCookieString = "";
                        return false;
                    }

                Product product = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(productId));
                if(product == null || prdQty <= 0)
                {
                    cartCookieString = "";
                    return false;
                }

                visitorCartList.Add(new CartDetail
                {
                    Product = product,
                    Quantity = prdQty
                }) ;
                cost += product.Price * prdQty;
            }
            totalCost = cost;
            return true;
        }

        public int GetCartQuantity()
        {
            return GetCartQty(cartCookieString);
        }

        //Static method to be used for situation with only cookie string
        public static int GetCartQty(string cookieString)
        {
            int qty = 0;
            string[] itemarray = cookieString.Split(itemSeperator);
            foreach(string itemstr in itemarray)
            {
                string[] itemdes = itemstr.Split(qtySeperator);
                if(itemdes.Length == 2)
                {
                    try
                    {
                        qty += Int32.Parse(itemdes[1]);
                    }
                    catch(Exception e)
                    {
                        continue;
                    }
                }
            }
            return qty;
        }

    }

}
