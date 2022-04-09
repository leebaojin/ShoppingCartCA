using Microsoft.AspNetCore.Mvc;
using ShoppingCartCA.Models;
using System;
using System.Linq;

namespace ShoppingCartCA.Controllers
{
    public class LogoutController : Controller
    {
        private readonly DBContext dbContext;

        public LogoutController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // remove session from our database
            if (Request.Cookies["SessionId"] != null && Request.Cookies["Username"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["sessionId"]);

                Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);
                if (session != null)
                {
                    // delete session record from our database;
                    dbContext.Remove(session);

                    // commit to save changes
                    dbContext.SaveChanges();
                }
            }

            // ask client to remove these cookies so that
            // they won't be sent over next time
            Response.Cookies.Delete("SessionId");
            Response.Cookies.Delete("Username");

            return RedirectToAction("Index", "Login");
        }
    }
}
