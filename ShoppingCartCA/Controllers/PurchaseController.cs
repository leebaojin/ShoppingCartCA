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

            Customer customer = dbContext.Customers.FirstOrDefault();

            //List<Order> orderList = account.Orders.ToList();
            ViewBag.customer = customer;
            //ViewBag.orderList = orderList;

            return View();

        }


    }
}
