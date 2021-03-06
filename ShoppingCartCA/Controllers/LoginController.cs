using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace ShoppingCartCA.Controllers
{
    public class LoginController : Controller
    {
        private DBContext dbContext;

        public LoginController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string loginattempt)
        {
            if (Request.Cookies["SessionId"] != null && Request.Cookies["Username"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["sessionId"]);
                Session session = dbContext.Sessions.FirstOrDefault(x =>
                    x.Id == sessionId
                );
                string userName = Request.Cookies["Username"];
                CustomerDetail customerDetails = dbContext.CustomerDetails.FirstOrDefault(x =>
                    x.Username == userName
                );

                if (session == null || userName == null)
                {
                    // someone has used an invalid Session ID (to fool us?); 
                    // route to Logout controller
                    return RedirectToAction("Index", "Logout");
                }

                // valid Session ID; route to Home page
                return RedirectToAction("Index", "Home");
            }
            if (loginattempt == "attempt1")
            {
                ViewData["loginfail"] = true;
            }
            else
            {
                ViewData["loginfail"] = false;
            }

            // no Session ID; show Login page
            return View();
        }

        public IActionResult Login(IFormCollection form)
        {
            string username = form["username"];
            string password = form["password"];

            HashAlgorithm sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(
                Encoding.UTF8.GetBytes(username + password));

            CustomerDetail customerDetail = dbContext.CustomerDetails.FirstOrDefault(x =>
                x.Username == username &&
                x.PassHash == hash
            );

            if (customerDetail == null)
            {
                return RedirectToAction("Index", "Login", new { loginattempt = "attempt1" });
            }

            Customer customer = dbContext.Customers.FirstOrDefault(x =>
                x.Id == customerDetail.CustomerId
            );

            // create a new session and tag to user
            Session session = new Session()
            {
                Customer = customer
            };
            dbContext.Sessions.Add(session);
            dbContext.SaveChanges();

            //Merge cart cookie to customer cart 
            MergeCart(customer);


            // ask browser to save and send back these cookies next time
            Response.Cookies.Append("SessionId", session.Id.ToString());
            Response.Cookies.Append("Username", customerDetail.Username);

            return RedirectToAction("Index", "Home");
        }

        private void MergeCart(Customer customer)
        {
            string cartCookie = Request.Cookies["shoppingcarttemp4"];
            if(cartCookie == null || cartCookie.Length == 0)
            {
                return;
            }
            if(customer == null)
            {
                return;
            }
            VisitorCart visitorCart = new VisitorCart(dbContext, cartCookie);
            visitorCart.PopulateList();
            List<CartDetail> cartList = visitorCart.VisitorCartList;
            foreach(CartDetail cd in cartList)
            {
                CartDetail customerCartDetail = customer.CartDetails.FirstOrDefault(x => x.Product.Id == cd.Product.Id);
                if(customerCartDetail == null)
                {
                    customer.CartDetails.Add(cd);
                }
                else
                {
                    customerCartDetail.Quantity += cd.Quantity;
                }
            }
            dbContext.SaveChanges();
            Response.Cookies.Delete("shoppingcarttemp4");
        }
    }
}
