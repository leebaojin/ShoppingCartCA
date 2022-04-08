using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;

namespace ShoppingCartCA.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext dbContext;

        public CartController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping", "Checkout" },false);

            ViewData["allcartitem"] = dbContext.CartDetails.Where(x => x.Customer.CustomerDetails.Username == "jeamsee").ToList();
            
            return View();
        }

        public IActionResult AddToCart(string productId, int qty=1)
        {
            //To autenticate the session

            //To create new cartdetail and add product in.

            //Return if successful and the new cart quantity
            return Json(new { updateSuccess = true, cartqty=1 });
        }

        public IActionResult UpdateCartItem(string cartItemId, int qty)
        {

            return Json(new { updateSuccess = true, cartqty = 1 });
        }

        public IActionResult RemoveCartItem(string cartItemId)
        {

            return Json(new { updateSuccess = true, cartqty = 1 });
        }
    }
}
