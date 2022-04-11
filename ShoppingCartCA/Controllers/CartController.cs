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
            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping" }, false);
            
            return View();
        }

        public IActionResult AddToCart([FromBody] DataCartProduct datacartproduct)
        {
            //To autenticate the session
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");

            bool succeed;
            int cartQty = 0;
            if (customer == null)
            {
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                VisitorCart visitorCart = new VisitorCart(dbContext, cartCookie);
                succeed = visitorCart.AddItem(datacartproduct.ProdId, datacartproduct.Quantity);
                cartQty = visitorCart.GetCartQuantity();
            }
            else
            {
                //Update cart
                succeed = CartData.AddToCart(customer, datacartproduct.ProdId, datacartproduct.Quantity, dbContext);
            }
            

            if (!succeed)
            {
                //Update failed
                return Json(new { addSuccess = false });
            }

            if (customer != null)
            {
                cartQty = CartData.GetCartSize(customer);
            }


                //Return if successful and the new cart quantity
                return Json(new { addSuccess = true, cartqty= cartQty});
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
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "lynnwong");

            if (prdId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Product product = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(prdId));
            if(product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Review cusReview = ReviewData.GetReview(product, customer);
            if(cusReview == null)
            {
                ViewData["canreview"] = ReviewData.CanReview(product, customer);
            }
            else
            {
                ViewData["canreview"] = true;
            }

            ViewData["productdisplay"] = product;
            ViewData["similarproduct"] = ProductData.GetSimilar(product);
            ViewData["review"] = cusReview;
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
