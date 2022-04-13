using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;

namespace ShoppingCartCA.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly DBContext dbContext;
        public CheckoutController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping" });

            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            if (customer == null || customer.CartDetails.Count == 0)
            {
                return RedirectToAction("Index", "Logout");
            }
            ViewData["allcartitem"] = customer.CartDetails;
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping" }, false);

            return View();
        }


        public IActionResult SubmitOrder()
        {
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);

            Customer customerProfile = dbContext.Customers.FirstOrDefault(customerProfile => customerProfile.Id == customer.Id);

            customerProfile.Orders.Add(GenerateOrder(customerProfile.CartDetails.ToList()));

            dbContext.SaveChanges();

            ClearCart(customerProfile.CartDetails.ToList());

            return View("Redirect");/*RedirectToAction("Index", "Purchase")*/;
        }

        private Order GenerateOrder(List<CartDetail> allcartitem)
        {
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                OrderDetails = GenerateOrderDetails(allcartitem)
            };

            return order;
        }

        private List<OrderDetail> GenerateOrderDetails(List<CartDetail> allcartitem)
        {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (CartDetail cartItem in allcartitem)
            {
                OrderDetail orderDetail = new OrderDetail
                {
                    Quantity = cartItem.Quantity,
                    Product = cartItem.Product,
                    PurchasePrice = cartItem.Product.Price,
                    ActivationCodes = GenerateActivationCode(cartItem.Quantity, cartItem.Product)
                };
                orderDetails.Add(orderDetail);
            }
            return orderDetails;
        }

        private List<ActivationCode> GenerateActivationCode(int productQuantity, Product product)
        {
            List<ActivationCode> activationCodes = new List<ActivationCode>();

            for (int i = 0; i < productQuantity; i++)
            {
                ActivationCode activationCode = new ActivationCode
                {
                    ProductId = product.Id
                };
                activationCodes.Add(activationCode);
            }
            return activationCodes;
        }

        private void ClearCart(List<CartDetail> allcartitem)
        {
            foreach (CartDetail cartItem in allcartitem)
            {
                dbContext.Remove(cartItem);
            }
            dbContext.SaveChanges();
        }

    }

}    
      
