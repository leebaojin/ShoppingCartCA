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
            if (customer == null)
            {
                return RedirectToAction("Index", "Logout");
            }
            ViewData["allcartitem"] = customer.CartDetails;
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping" }, false);

            return View();
        }

        public IActionResult AddToCart([FromBody] DataCartProduct datacartproduct)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(datacartproduct.ProdId);
            }
            catch (Exception e)
            {
                return Json(new { addSuccess = false });
            }
            if (datacartproduct.Quantity <= 0)
            {
                return Json(new { addSuccess = false });
            }

            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);

            if (customer == null)
            {
                return Json(new { addSuccess = false });
            }

            CartDetail cartDetail = customer.CartDetails.FirstOrDefault(x => x.Product.Id == productId);
            if (cartDetail == null)
            {
                Product newProduct = dbContext.Products.FirstOrDefault(x => x.Id == productId);
                if (newProduct == null)
                {
                    return Json(new { addSuccess = false });
                }
                //To create new cartdetail and add product in.
                customer.CartDetails.Add(new CartDetail()
                {
                    Product = newProduct,
                    Quantity = datacartproduct.Quantity
                });
            }
            else
            {
                cartDetail.Quantity += datacartproduct.Quantity;
            }
            dbContext.SaveChanges();

            return Json(new { addSuccess = true, cartqty = CartData.GetCartSize(customer) });

        }

    }    
        
}
