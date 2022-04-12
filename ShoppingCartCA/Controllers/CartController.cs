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

            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "lynnwong");

            Product product = FindProduct(prdId);
            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if(customer == null)
            {
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "My Purchase" }, true, cartCookie);
            }
            else
            {
                ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping" }, true);
            }

            Review cusReview = ReviewData.GetReview(product, customer);
            if(cusReview == null)
            {
                //check if can perform review
                ViewData["canreview"] = ReviewData.CanReview(product, customer);
                ViewData["allreview"] = ReviewData.GetAllReviewExcept(product);
            }
            else
            {
                ViewData["canreview"] = true;
                ViewData["allreview"] = ReviewData.GetAllReviewExcept(product, cusReview);
            }

            ViewData["productdisplay"] = product;
            ViewData["similarproduct"] = ProductData.GetSimilar(product);
            ViewData["review"] = cusReview;
            return View();
        }

        [Route("Home/SendReview")]
        public IActionResult SendReview([FromBody] DataReviewPost dataReviewPost)
        {
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "lynnwong");

            Product product = FindProduct(dataReviewPost.PrdId);
            if (product == null)
            {
                return Json(new { postSuccess = false });
            }

            bool success = false;
            Review cusReview = null;
            if (dataReviewPost.CommentId != "0")
            {
                //For those with a commentId sent
                try
                {
                    cusReview = dbContext.Reviews.FirstOrDefault(x => x.Id == Guid.Parse(dataReviewPost.CommentId) && x.Customer.Id == customer.Id && x.Product.Id == Guid.Parse(dataReviewPost.PrdId));
                    if (cusReview == null)
                    {
                        //If wrong Id, return fail
                        return Json(new { postSuccess = false });
                    }

                }
                catch (Exception e)
                {
                    return Json(new { postSuccess = false });
                }
                success = UpdateReview(cusReview, dataReviewPost);
            }
            else
            {
                cusReview = ReviewData.GetReview(product, customer);
                if (cusReview == null)
                {
                    if (!ReviewData.CanReview(product, customer))
                    {
                        return Json(new { postSuccess = false });
                    }
                    dbContext.Add(new Review()
                    {
                        Product = product,
                        Customer = customer,
                        Comment = dataReviewPost.ReviewMsg,
                        Rating = dataReviewPost.ReviewRating
                    });
                    dbContext.SaveChanges();
                    success = true;
                }
                else
                {
                    success = UpdateReview(cusReview, dataReviewPost);
                }
            }

            return Json ( new {postSuccess = success, starRating = dataReviewPost.ReviewRating});
        }

        private Product FindProduct(string prdId)
        {
            if (prdId == null)
            {
                return null;
            }
            return dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(prdId));

        }

        private bool UpdateReview(Review review, DataReviewPost dataReviewPost)
        {
            if(dataReviewPost.ReviewMsg.Length == 0 || dataReviewPost.ReviewRating < -1 || dataReviewPost.ReviewRating>5)
            {
                return false;
            }
            review.Comment = dataReviewPost.ReviewMsg;
            review.Rating = dataReviewPost.ReviewRating;
            dbContext.SaveChanges();
            return true;

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
