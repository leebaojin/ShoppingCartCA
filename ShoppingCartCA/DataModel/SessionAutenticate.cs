﻿using System;
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
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.SessionId == Guid.Parse(sessionId));

            return customer;
        }
    }
}
