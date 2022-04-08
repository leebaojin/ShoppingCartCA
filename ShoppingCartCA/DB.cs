using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            this.env = env;
        }

        public void Seed()
        {
            SeedProduct();
            SeedAccount();
            
        }

        private void SeedAccount()
        {
            if (dbContext.Accounts.Any())
            {
                return;   // DB has been seeded
            }

            HashAlgorithm sha = SHA256.Create();

            string[] usernames = { "jeamsee", "lynnwong", "leliamay" };

            string[] FirstName = { "Jeam", "Lynn", "Lelia" };

            string[] LastName = { "See", "Wong", "May" };

            string password = "mysecret";

            int i = 0;

            foreach (string username in usernames)
            {
                string combo = username + password;
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));

                dbContext.Add( new Cart()
                {
                    Account = new Account()
                    {
                        Username = username,
                        PassHash = hash,
                        FirstName = FirstName[i],
                        LastName = LastName[i],

                    }
                }                   
                    );

                i++;

                
            }
            dbContext.SaveChanges();
        }


        private void SeedProduct()
        {
            //string path = Path.Combine(env.WebRootPath,"pictures");
            //string path2 = Path.Combine(env.WebRootPath,"document");
            string path = "/pictures";
            string path2 = "/document";

            dbContext.Add(new Product() {
                Name = ".NET Charts",
                Desc = "Brings powerful charting capabilities to your .NET applications.",
                Price = 99,
                Img = path + "/dotnet_charts_00da522c-3653-491c-8749-7fd8e71ad728.jpg",
                DownloadFile = path2+ "/dotnet_charts_395ab7e2-2fd0-4629-911c-4f8ddc0858b5.pdf"
            }) ;

            dbContext.Add(new Product()
            {
                Name = ".NET PayPal",
                Desc = "Integrate your .NET apps with PayPal the easy way!",
                Price = 69,
                Img = path+ "/dotnet_paypal_b87f5d05-ecba-440d-b43c-5c44b8a19852.jpg",
                DownloadFile = path2+ "/dotnet_paypal_9163eb84-200f-4225-86e1-bf96637bf0e5.pdf"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET ML",
                Desc = "Supercharged .NET machine learning libraries.",
                Price = 299,
                Img = path+ "dotnet_ml_24a58381-f610-400a-9b8d-c4bc290fc46e.jpg",
                DownloadFile = path2+ "dotnet_ml_b99867c4-75c8-4b8f-b7ea-08e94637b45a.pdf"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Analytics",
                Desc = "Performs data mining and analytics easily in .NET.",
                Price = 299,
                Img = path+ "dotnet_analytics_88ac7cbd-80da-4ff2-a1a5-a324bf648f15.jpg",
                DownloadFile = path2+ "dotnet_analytics_33c7b0ed-75f3-47ba-89d5-56b674c5e5de.pdf"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Logger",
                Desc = "Logs and aggregates events easily in your .NET apps.",
                Price = 49,
                Img = path+ "dotnet_logger_c45f20ff-8adc-4da3-8c60-ae8c829bc832.jpg",
                DownloadFile = path2+ "dotnet_logger_d07f437e-5edc-406d-b52d-0c42a55f16e3.pdf"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Numerics",
                Desc = "Powerful numerical methods for your .NET simulations.",
                Price = 199,
                Img = path+ "notnet_numerics_f43b191e-a6ba-4d3b-8d37-8225686c5ca1.jpg",
                DownloadFile = path2+ "dotnet_numerics_76d3a7b5-b44e-46b3-950a-3957cb4aa6b4.pdf"
            });

            dbContext.SaveChanges();
        }

    }
}
