using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;
using System.Diagnostics;

namespace ShoppingCartCA.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly DBContext dbContext;

        public PurchaseController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping"});

            //Customer customer = dbContext.Customers.FirstOrDefault();

            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            if (customer == null)
            {
                return RedirectToAction("Index", "Logout");
            }
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping"}, true);
            ViewBag.customer = customer;
 
            return View();
        }
    }
}
