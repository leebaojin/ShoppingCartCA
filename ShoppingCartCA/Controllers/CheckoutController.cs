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
        
        public IActionResult ConvertOrder()
        {
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            if(customer == null || customer.CartDetails.Count == 0)
            {
                return RedirectToAction("Index", "Logout");
            }

            //Create new order
            Order createOrder = new Order()
            {
                OrderDate = DateTime.Now
            };

            foreach(CartDetail cd in customer.CartDetails)
            {
                OrderDetail od = new OrderDetail()
                {
                    Product = cd.Product,
                    Quantity = cd.Quantity,
                    PurchasePrice = cd.Product.Price
                };
                for(int i = 0; i < od.Quantity; i++)
                {
                    od.ActivationCodes.Add(new ActivationCode());
                }
                createOrder.OrderDetails.Add(od);
            }
            customer.CartDetails.Clear();
            customer.Orders.Add(createOrder);

            dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        
    }

}    
      
