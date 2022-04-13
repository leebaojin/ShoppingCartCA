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
            Customer customer = SessionAutenticate.Autenticate(sessionId, dbContext);

            if (customer != null)
            {
                ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "My Purchase" });
            }
            else
            {
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "My Purchase" },true, cartCookie);
            }

            if (searchStr == null)
            {
                searchStr = "";
            }
            List<Product> products = dbContext.Products.Where(x =>
                                    x.Name.Contains(searchStr) || x.Desc.Contains(searchStr)
                                    ).ToList();
            
            ViewBag.products = products;
            ViewData["searchStr"] = searchStr;
            return View();
        }

        public IActionResult Search()
        {
            return RedirectToAction("Login");
        }

        public IActionResult ProdDetail(string prdId)
        {
            //Use of prdId instead of prodId to avoid clash with the AddToCart

            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);

            Product product = FindProduct(prdId);
            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (customer == null)
            {
                string cartCookie = Request.Cookies["shoppingcarttemp4"];
                ViewData["layoutheader"] = new LayoutHeader(null, new string[] { "Continue Shopping" }, true, cartCookie);
            }
            else
            {
                ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "My Purchase", "Continue Shopping" }, true);
            }

            Review cusReview = ReviewData.GetReview(product, customer);
            if (cusReview == null)
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

        public IActionResult SendReview([FromBody] DataReviewPost dataReviewPost)
        {
            Customer customer = SessionAutenticate.Autenticate(Request.Cookies["SessionId"], dbContext);

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

            return Json(new { postSuccess = success, starRating = dataReviewPost.ReviewRating });
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
            if (dataReviewPost.ReviewMsg.Length == 0 || dataReviewPost.ReviewRating < -1 || dataReviewPost.ReviewRating > 5)
            {
                return false;
            }
            review.Comment = dataReviewPost.ReviewMsg;
            review.Rating = dataReviewPost.ReviewRating;
            dbContext.SaveChanges();
            return true;
        }
    }
}
