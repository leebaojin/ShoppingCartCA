using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using ShoppingCartCA.Models;

namespace ShoppingCartCA
{
    public class DB
    {
        private readonly DBContext dbContext;

        private readonly IWebHostEnvironment env;

        public DB(DBContext dbContext, IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
        }

        public void Seed()
        {

        }

        private void SeedAccount()
        {

        }

        private void SeedProduct()
        {

        }
    }
}
