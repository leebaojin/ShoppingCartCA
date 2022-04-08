using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class LayoutHeader
    {
        public string User { get; set; }
        public List<HeaderLink> HeaderLinks { get; set; }
        public HeaderLink CartLink { get; set; }
        public int CartSize { get; set; }

        public LayoutHeader(Customer customer, string[] headerlink, bool cartview=true)
        {
            if(customer == null)
            {
                User = "Guest";
                CartSize = 0;
            }
            else
            {
                if(customer.Account == null)
                {
                    User = "Guest";
                }
                else
                {
                    User = customer.Account.FirstName + customer.Account.LastName;
                }
                CartSize = customer.CartDetails.Count;
            }
            HeaderLinks = new List<HeaderLink>();
            CreateHeaders(headerlink);
            if (cartview)
            {
                CartLink = CreateCart();
            }
        }

        private void CreateHeaders(string[] headerlink)
        {
            foreach(string header in headerlink)
            {
                switch(header)
                {
                    case "Login":
                        HeaderLinks.Add(CreateLogin());
                        break;
                    case "Logout":
                        HeaderLinks.Add(CreateLogout());
                        break;
                    case "Continue Shopping":
                        HeaderLinks.Add(CreateShopping());
                        break;
                    case "Checkout":
                        HeaderLinks.Add(CreateCheckout());
                        break;
                    case "My Purchase":
                        HeaderLinks.Add(CreateViewPurchase());
                        break;
                    case "My Cart":
                        HeaderLinks.Add(CreateCart());
                        break;
                }
            }
        }

        private HeaderLink CreateLogin()
        {
            return new HeaderLink()
            {
                Title = "Login",
                Controller = "Login",
                Action = "Index"
            };
        }

        private HeaderLink CreateLogout()
        {
            return new HeaderLink()
            {
                Title = "Logout",
                Controller = "Login",
                Action = "Logout"
            };
        }

        private HeaderLink CreateShopping()
        {
            return new HeaderLink()
            {
                Title = "Continue Shopping",
                Controller = "Home",
                Action = "Index"
            };
        }

        private HeaderLink CreateCart()
        {
            return new HeaderLink()
            {
                Title = "My Cart",
                Controller = "Cart",
                Action = "Index"
            };
        }

        private HeaderLink CreateCheckout()
        {
            return new HeaderLink()
            {
                Title = "Checkout",
                Controller = "Checkout",
                Action = "Index"
            };
        }

        private HeaderLink CreateViewPurchase()
        {
            return new HeaderLink()
            {
                Title = "My Purchase",
                Controller = "MyPurchase",
                Action = "Index"
            };
        }


    }

    public class HeaderLink
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}
