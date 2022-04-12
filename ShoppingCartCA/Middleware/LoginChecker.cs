using Microsoft.AspNetCore.Http;
using ShoppingCartCA.Models;
using System.Threading.Tasks;

namespace ShoppingCartCA.Middleware
{
    public class LoginChecker
    {
        private readonly RequestDelegate next;
        private readonly DBContext dbContext;

        public LoginChecker(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string sessionId = context.Request.Cookies["sessionId"];

            if (sessionId == null)
            {
                context.Response.Redirect("/Login");
                return;
            }

            await next(context);
        }
    }
}