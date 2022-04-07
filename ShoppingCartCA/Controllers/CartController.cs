using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            //Changes were made
            return View();
        }
    }
}
