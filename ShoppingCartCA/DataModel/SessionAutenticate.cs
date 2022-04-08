using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class SessionAutenticate
    {
        public static Cart Autenticate(string sessionId, DBContext dbContext)
        {
            Cart cart = dbContext.Carts.FirstOrDefault(x => x.Session.Id == Guid.Parse(sessionId));

            return cart;
        }
    }
}
