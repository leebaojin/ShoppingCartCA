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
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                if(cartCookie == null)
                {
                    return RedirectToAction("Index", "Logout");
                }
                VisitorCart visitorCart = new VisitorCart(dbContext, cartCookie);
                if(visitorCart.PopulateList())
                {
                    return RedirectToAction("Index", "Logout");
                }
                ViewData["customerknown"] = false;
                ViewData["allcartitem"] = visitorCart.VisitorCartList;
                ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping" }, false, cartCookie);
            }
            else
            {
                ViewData["customerknown"] = true;
                ViewData["allcartitem"] = customer.CartDetails;
                ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping" }, false);
            }

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

                //Modify Cookie (regardless of success / fail)
                //Possible cookie becomes empty if there is a problem in the format
                Response.Cookies.Append("shoppingcarttemp4", visitorCart.cartCookieString);
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
            ProcessCart processCart;
            if (customer == null)
            {
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                processCart = UpdateCartCookie(cartUpdateData, cartCookie);
            }
            else
            {
                processCart = UpdateCartDatabase(cartUpdateData, customer);
            }
             
            if (!processCart.Success)
            {
                //For failure to update correctly
                return Json(new {updateSuccess = false});
            }
            

            if (cartUpdateData.Newqty > 0)
            {
                //If the row is not deleted
                return Json(new
                {
                    updateSuccess = true,
                    updateRow = cartUpdateData.UpdateRow,
                    newqty = cartUpdateData.Newqty,
                    price = String.Format("{0:0.00}", processCart.DetailPrice),
                    totalprice = String.Format("{0:0.00}", processCart.TotalPrice)
                });
            }
            return Json(new
            {
                updateSuccess = true,
                updateRow = cartUpdateData.UpdateRow,
                removeItem = true,
                totalprice = String.Format("{0:0.00}", processCart.TotalPrice)
            });
        }

        private ProcessCart UpdateCartDatabase(DataCartUpdate cartUpdateData, Customer customer)
        {
            Guid cartIdToUpdate;
            try
            {
                cartIdToUpdate = Guid.Parse(cartUpdateData.CartItemId);
            }
            catch (Exception e)
            {
                return new ProcessCart(false,0,0);
            }
            bool cartNotUpdated = true;
            double totalCost = 0; double detailCost = 0;

            if (cartUpdateData.Newqty <= 0)
            {
                CartDetail cartDetailDelete = customer.CartDetails.FirstOrDefault(x => x.Id == cartIdToUpdate);
                if (cartDetailDelete == null)
                {
                    return new ProcessCart(false, 0, 0);
                }
                customer.CartDetails.Remove(cartDetailDelete);
                cartNotUpdated = false;
            }
            if(customer.CartDetails.Count == 0)
            {
                //To reload page if cart is empty
                return new ProcessCart(false, 0, 0);
            }

            foreach (CartDetail cartDetail in customer.CartDetails)
            {
                if (cartNotUpdated && cartDetail.Id == cartIdToUpdate)
                {
                    cartDetail.Quantity = cartUpdateData.Newqty;
                    cartNotUpdated = false;
                    detailCost = cartDetail.Quantity * cartDetail.Product.Price;
                }
                totalCost += cartDetail.Quantity * cartDetail.Product.Price;
            }

            if (cartNotUpdated)
            {
                return new ProcessCart(false, 0, 0);
            }
            dbContext.SaveChanges();

            return new ProcessCart(true, detailCost, totalCost);
        }

        private ProcessCart UpdateCartCookie(DataCartUpdate cartUpdateData, string cartCookie)
        {
            if(cartCookie == null || cartCookie.Length == 0)
            {
                return new ProcessCart(false, 0, 0);
            }
            VisitorCart visitorCart = new VisitorCart(dbContext, cartCookie);
            if(visitorCart.UpdateItem(cartUpdateData.ProdItemId, cartUpdateData.Newqty))
            {
                //there is some issue with updating item
                //note that the value inside the visitorcart is updated
                Response.Cookies.Append("shoppingcarttemp4", visitorCart.cartCookieString);
                return new ProcessCart(false, 0, 0);
            }
            if(visitorCart.cartCookieString == "")
            {
                //Reload page if the cart is empty
                return new ProcessCart(false, 0, 0);
            }
            if (!visitorCart.PopulateList())
            {
                //there is some issue in the cookie
                //note that the value inside the visitorcart is updated
                Response.Cookies.Append("shoppingcarttemp4", visitorCart.cartCookieString);
                return new ProcessCart(false, 0, 0);
            }
            double updatePrice = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(cartUpdateData.ProdItemId)).Price * cartUpdateData.Newqty;

            Response.Cookies.Append("shoppingcarttemp4", visitorCart.cartCookieString);
            return new ProcessCart(false, updatePrice, visitorCart.TotalCost);
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
