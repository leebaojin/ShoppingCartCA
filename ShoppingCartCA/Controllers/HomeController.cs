using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCartCA.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.DataModel;

namespace ShoppingCartCA.Controllers
{
    public class HomeController : Controller
    {
        private DBContext dbContext;

        public HomeController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string searchStr)
        {
			string sessionId = Request.Cookies["SessionId"];

            if(sessionId != null)
            {
                Customer customer = SessionAutenticate.Autenticate(sessionId, dbContext);

                ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "My Cart" });
            }
            else
            {
                ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "My Cart" });
            }

            if (searchStr == null)
            {
                searchStr = "";
            }
            List<Product> products = dbContext.Products.Where(x =>
                                    x.Name.Contains(searchStr)
                                    ).ToList();
            
            ViewBag.products = products;
            ViewData["searchStr"] = searchStr;
            return View();
        }

        public IActionResult Search()
        {
            return RedirectToAction("Login");
        }
    }
}
