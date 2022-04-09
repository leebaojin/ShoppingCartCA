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

        public LayoutHeader(Customer customer, string[] headerlink=null, bool cartview=true)
        {
            HeaderLinks = new List<HeaderLink>();
            if (customer == null)
            {
                User = "Guest";
                CartSize = 0;
                CreateHeaderLogin("Login");
            }
            else
            {
                if(customer.CustomerDetails == null)
                {
                    User = "Guest";
                    CreateHeaderLogin("Login");
                }
                else
                {
                    User = customer.CustomerDetails.FirstName + " " + customer.CustomerDetails.LastName;
                    CreateHeaderLogin("Logout");
                }
                CartSize = CartData.GetCartSize(customer);
            }
            CreateHeaders(headerlink, customer);
            if (cartview)
            {
                CartLink = CreateCart();
            }
        }
        private void CreateHeaderLogin(string headerlink)
        {
            switch (headerlink)
            {
                case "Login":
                    HeaderLinks.Add(CreateLogin());
                    break;
                case "Logout":
                    HeaderLinks.Add(CreateLogout());
                    break;
            }
        }

        private void CreateHeaders(string[] headerlink, Customer customer)
        {
            if(headerlink == null)
            {
                return;
            }
            foreach(string header in headerlink)
            {
                switch(header)
                {
                    case "Continue Shopping":
                        HeaderLinks.Add(CreateShopping());
                        break;
                    case "Checkout":
                        HeaderLinks.Add(CreateCheckout());
                        break;
                    case "My Purchase":
                        if(customer != null && customer.CustomerDetails != null)
                        {
                            HeaderLinks.Add(CreateViewPurchase());
                        } 
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
                Controller = "Logout",
                Action = "Index"
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
                Controller = "Purchase",
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
