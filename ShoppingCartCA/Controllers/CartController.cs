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
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            if(customer == null)
            {
                return RedirectToAction("Index", "Logout");
            }
            ViewData["allcartitem"] = customer.CartDetails;
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping", "Checkout" }, false);
            
            return View();
        }

        public IActionResult AddToCart([FromBody] DataCartProduct datacartproduct)
        {
            Guid productId;
            try
            {
                productId = Guid.Parse(datacartproduct.ProdId);
            }
            catch(Exception e)
            {
                return Json(new { addSuccess = false });
            }
            if(datacartproduct.Quantity <= 0)
            {
                return Json(new { addSuccess = false });
            }
            
            //To autenticate the session
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");

            //Check if there is already this product
            CartDetail cartDetail = customer.CartDetails.FirstOrDefault(x => x.Product.Id == productId);

            if(cartDetail == null)
            {
                Product newProduct = dbContext.Products.FirstOrDefault(x => x.Id == productId);
                if(newProduct == null)
                {
                    return Json(new { addSuccess = false });
                }
                //To create new cartdetail and add product in.
                customer.CartDetails.Add(new CartDetail()
                {
                    Product = newProduct,
                    Quantity = datacartproduct.Quantity
                }) ;
            }
            else
            {
                cartDetail.Quantity += datacartproduct.Quantity;
            }
            dbContext.SaveChanges();

            //Return if successful and the new cart quantity
            return Json(new { addSuccess = true, cartqty= CartData.GetCartSize(customer) });
        }

        public IActionResult UpdateCartItem([FromBody] DataCartUpdate cartUpdateData )
        {
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            if (customer == null)
            {
                return RedirectToAction("Index", "Logout");
            }
            Guid cartIdToUpdate;
            try
            {
                cartIdToUpdate = Guid.Parse(cartUpdateData.CartItemId);
            }
            catch (Exception e)
            {
                return Json(new { updateSuccess = false });
            }
            bool cartNotUpdated = true;
            double totalcost = 0; double detailCost = 0;

            if(cartUpdateData.Newqty <= 0)
            {
                CartDetail cartDetailDelete = customer.CartDetails.FirstOrDefault(x => x.Id == cartIdToUpdate);
                if(cartDetailDelete == null)
                {
                    return Json(new { updateSuccess = false });
                }
                customer.CartDetails.Remove(cartDetailDelete);
               cartNotUpdated = false;
            }
            
            foreach (CartDetail cartDetail in customer.CartDetails)
            {
                if(cartNotUpdated && cartDetail.Id == cartIdToUpdate)
                {
                    cartDetail.Quantity = cartUpdateData.Newqty;
                    cartNotUpdated = false;
                    detailCost = cartDetail.Quantity * cartDetail.Product.Price;
                }
                totalcost += cartDetail.Quantity * cartDetail.Product.Price;
            }
           
            if (cartNotUpdated)
            { 
                return Json(new { updateSuccess = false }); 
            }
            dbContext.SaveChanges();

            if (cartUpdateData.Newqty > 0)
            {
                //If the row is not deleted
                return Json(new
                {
                    updateSuccess = true,
                    updateRow = cartUpdateData.UpdateRow,
                    newqty = cartUpdateData.Newqty,
                    price = String.Format("{0:0.00}", detailCost),
                    totalprice = String.Format("{0:0.00}", totalcost)
                });
            }
            return Json(new
            {
                updateSuccess = true,
                updateRow = cartUpdateData.UpdateRow,
                removeItem = true,
                totalprice = String.Format("{0:0.00}", totalcost)
            });
        }

        [Route("Home/ProdDetail")]
        public IActionResult ProdDetail(string prdId)
        {
            //Use of prdId instead of prodId to avoid clash with the AddToCart

            //Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            if (prdId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Product product = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(prdId));
            if(product == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["productdisplay"] = product;
            ViewData["similarproduct"] = ProductData.GetSimilar(product);
            ViewData["review"] = null;
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping", "My Cart" }, true);
            return View();
        }

        public IActionResult Testing()
        {
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "My Cart", "My Purchase" });
            
            ViewData["cartdata"] = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Charts");
            return View("template");
        }

    }
}
