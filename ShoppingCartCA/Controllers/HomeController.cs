using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCartCA.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Controllers
{
    public class HomeController : Controller
    {
        private DBContext dbContext;

        public HomeController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            ViewData["layoutheader"] = new LayoutHeader(null,new string[]{ "Login", "My Cart" });
            return View();
        }

        public IActionResult Search()
        {
            return RedirectToAction("Login");
        }
    }
}
