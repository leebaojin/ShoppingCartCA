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
            if(sessionId == null)
            {
                return null;
            }
            Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == Guid.Parse(sessionId));
            Customer customer = (session == null) ? null : session.Customer;
            /*
            if(customer.CustomerDetails != null)
            {
                if(username != customer.CustomerDetails.Username)
                {
                    return null;
                }
            }else if(username != null)
            {
                //if there is a username when customer has no details. 
                return null;
            }*/

            return customer;
        }
    }
}
