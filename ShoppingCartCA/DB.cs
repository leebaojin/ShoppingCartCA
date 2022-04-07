using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA
{
    public class DB
    {
        private DBContext dbContext;
        public DB(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Seed()
        {

        }
    }
}
