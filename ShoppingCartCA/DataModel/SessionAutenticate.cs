using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class SessionAutenticate
    {
        public static Customer Autenticate(string sessionId, DBContext dbContext)
        {
            //Customer customer = dbContext.Customers.FirstOrDefault(x => x.Session.Id == Guid.Parse(sessionId));

            Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == Guid.Parse(sessionId));
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.Id == session.Customer.Id);

            return customer;
        }
    }
}
