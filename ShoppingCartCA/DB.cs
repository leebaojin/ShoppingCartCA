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
            SeedCartDetails();
            SeedOrderAndOrderDetailAndActivationCode();
            SeedSimilarProduct();
            SeedComments();
            SeedDemoAccount();
        }

        private void SeedDemoAccount()
        {
            HashAlgorithm sha = SHA256.Create();
            string username = "petertan";
            string firstName = "Peter";
            string lastName = "Tan";
            string password = "mysecret";

            string combo = username + password;
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));
            Customer customer = new Customer() {
                CustomerDetails = new CustomerDetail()
                {
                    Username = username,
                    PassHash = hash,
                    FirstName = firstName,
                    LastName = lastName
                }
            };
            dbContext.Add(customer);
            dbContext.SaveChanges();
            Product product = dbContext.Products.FirstOrDefault(x => x.Name == ".NET CHARTS");

            Order newOrder = new Order()
            {
                OrderDate = new DateTime(2022, 4, 1),

            };
            OrderDetail newOrderDetail = new OrderDetail()
            {
                Quantity = 1,
                Product = product,
                PurchasePrice = product.Price,
            };
            ActivationCode activationCode = new ActivationCode()
            {

            };
            newOrderDetail.ActivationCodes.Add(activationCode);
            newOrder.OrderDetails.Add(newOrderDetail);
            customer.Orders.Add(newOrder);
            dbContext.SaveChanges();
        }

        private void SeedAccount()
        {
            HashAlgorithm sha = SHA256.Create();

            string[] usernames = { "jeamsee", "lynnwong", "leliamay","jameslow" };

            string[] FirstName = { "Jeam", "Lynn", "Lelia","James" };

            string[] LastName = { "See", "Wong", "May","Low"};

            string password = "mysecret";

            int i = 0;

            foreach (string username in usernames)
            {
                string combo = username + password;
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));
                dbContext.Add(new Customer()
                {
                    CustomerDetails = new CustomerDetail()
                    {
                        Username = username,
                        PassHash = hash,
                        FirstName = FirstName[i],
                        LastName = LastName[i],
                    }
                });
                i++;
            }
            dbContext.SaveChanges();
        }

        private void SeedCartDetails()
        {
            Customer myCustomer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            myCustomer.CartDetails.Add(new CartDetail()
            {
                Product = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Charts"),
                Quantity = 2
            });

            myCustomer.CartDetails.Add(new CartDetail()
            {
                Product = dbContext.Products.FirstOrDefault(x => x.Name == ".NET ML"),
                Quantity = 1
            });

            myCustomer.CartDetails.Add(new CartDetail()
            {
                Product = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Logger"),
                Quantity = 3
            });
            dbContext.SaveChanges();
        }
        private void SeedProduct()
        {
            //string path = Path.Combine(env.WebRootPath,"pictures");
            //string path2 = Path.Combine(env.WebRootPath,"document");
            string path = "/pictures";
            string path2 = "/document";

            dbContext.Add(new Product()
            {
                Name = ".NET Charts",
                Desc = "Brings powerful charting capabilities to your .NET applications.",
                Price = 99,
                Img = path + "/dotnet_charts_00da522c-3653-491c-8749-7fd8e71ad728.jpg",
                DownloadFile = path2 + "/dotnet_charts_395ab7e2-2fd0-4629-911c-4f8ddc0858b5.pdf",
                DownloadName = "dotNet_Charts"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET PayPal",
                Desc = "Integrate your .NET apps with PayPal the easy way!",
                Price = 69,
                Img = path + "/dotnet_paypal_b87f5d05-ecba-440d-b43c-5c44b8a19852.jpg",
                DownloadFile = path2 + "/dotnet_paypal_9163eb84-200f-4225-86e1-bf96637bf0e5.pdf",
                DownloadName = "dotNet_PayPal"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET ML",
                Desc = "Supercharged .NET machine learning libraries.",
                Price = 299,
                Img = path + "/dotnet_ml_24a58381-f610-400a-9b8d-c4bc290fc46e.jpg",
                DownloadFile = path2 + "/dotnet_ml_b99867c4-75c8-4b8f-b7ea-08e94637b45a.pdf",
                DownloadName = "dotNet_MachineLearning"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Analytics",
                Desc = "Performs data mining and analytics easily in .NET.",
                Price = 299,
                Img = path + "/dotnet_analytics_88ac7cbd-80da-4ff2-a1a5-a324bf648f15.jpg",
                DownloadFile = path2 + "/dotnet_analytics_33c7b0ed-75f3-47ba-89d5-56b674c5e5de.pdf",
                DownloadName = "dotNet_Analytics"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Logger",
                Desc = "Logs and aggregates events easily in your .NET apps.",
                Price = 49,
                Img = path + "/dotnet_logger_c45f20ff-8adc-4da3-8c60-ae8c829bc832.jpg",
                DownloadFile = path2 + "/dotnet_logger_d07f437e-5edc-406d-b52d-0c42a55f16e3.pdf",
                DownloadName = "dotNet_Logger"
            });

            dbContext.Add(new Product()
            {
                Name = ".NET Numerics",
                Desc = "Powerful numerical methods for your .NET simulations.",
                Price = 199,
                Img = path + "/notnet_numerics_f43b191e-a6ba-4d3b-8d37-8225686c5ca1.jpg",
                DownloadFile = path2 + "/dotnet_numerics_76d3a7b5-b44e-46b3-950a-3957cb4aa6b4.pdf",
                DownloadName = "dotNet_Numerics"
            });

            dbContext.SaveChanges();
        }

        public void SeedSimilarProduct()
        {
            Product chart = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Charts");
            Product analytics = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Analytics");
            Product numerics = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Numerics");
            Product ml = dbContext.Products.FirstOrDefault(x => x.Name == ".NET ML");
            Product logger = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Logger");

            chart.SimilarProducts.Add(analytics);
            chart.SimilarProducts.Add(numerics);
            numerics.SimilarProducts.Add(analytics);

            ml.SimilarProducts.Add(logger);

            dbContext.SaveChanges();
        }

        public void SeedComments()
        {
            Customer customer1 = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");

            List<Customer> customerList = dbContext.Customers.Where(x => x.CustomerDetails.Username != "jeamsee").ToList();
            Random rnd = new Random();
            
            Product productToAdd1 = customer1.Orders.ToList()[0].OrderDetails.ToList()[0].Product;

            string[] commentlist = new string[] {"Poor quanlity. Always crashes. Going to delete.",
                "The programme feels unstable with bugs.", "Overall ok but the UI is not very firendly.", "Good product. Would recommend it",
            "Excellent product. It feels naturally with easy to use features. Must Buy."};

            string[] commentlist2 = new string[] {"Not very good. Somehow seems broken.",
                "Won't recommend it unless you like a steep learning curve.", "Ok usable. Not a bad product", "Nice to use. Try it out.",
            "A good bargin! UI is so intuitive and all the necessary features are there."};

            Product myProduct = customer1.Orders.ToList()[0].OrderDetails.ToList()[0].Product;
            myProduct.Reviews.Add(new Review()
            {
                Customer = customer1,
                Rating = 3,
                Comment = commentlist[2]
            });
            dbContext.SaveChanges();
            foreach(Customer customerN in customerList) 
            {
                foreach (Order order in customerN.Orders.ToList())
                {
                    foreach (OrderDetail od in order.OrderDetails.ToList())
                    {
                        int rating = rnd.Next(1, 5);
                        int cmt = rnd.Next(1, 2);
                        int timediff = rnd.Next(1, 60);
                        string cmtstr;
                        if(cmt == 1)
                        {
                            cmtstr = commentlist[rating - 1];
                        }
                        else
                        {
                            cmtstr = commentlist2[rating - 1];
                        }
                        Review myreview = dbContext.Reviews.FirstOrDefault(x => x.Product.Id == od.Product.Id && x.Customer.Id == customerN.Id);
                        if (myreview == null)
                        {
                            Review reviewToAdd = new Review()
                            {
                                Customer = customerN,
                                Rating = rating,
                                Comment = cmtstr
                            };
                            reviewToAdd.Timestamp -= (timediff * 86400); //1 to 60 days difference
                            od.Product.Reviews.Add(reviewToAdd);
                        }
                        dbContext.SaveChanges();
                    }
                }
            }
            
            
        }


        private void SeedOrderAndOrderDetailAndActivationCode()
        {
            List<Customer> listOfCustomer = dbContext.Customers.ToList();

            foreach (Customer c in listOfCustomer)
            {
                Random random = new Random();
                int numOfOrdersToGenerate = random.Next(2, 4);
                c.Orders = RandomOrder(numOfOrdersToGenerate);
            }

            dbContext.SaveChanges();
        }

        private List<Order> RandomOrder(int numOfOrdersToGenerate)
        {
            List<Order> listOfOrder = new List<Order>();

            Random random = new Random();
            int day = random.Next(1, 20);

            for (int i = 0; i < numOfOrdersToGenerate; i++)
            {
                int numOfOrderDetailToGenerate = random.Next(1, 3);
                int month = random.Next(1, 4);

                Order order = new Order
                {
                    OrderDate = new DateTime(2022, month, day),
                    OrderDetails = RandomOrderDetail(numOfOrderDetailToGenerate)
                };
                listOfOrder.Add(order);

                day++;
            }

            return listOfOrder;
        }

        private List<OrderDetail> RandomOrderDetail(int numOfOrderDetailToGenerate)
        {
            List<OrderDetail> listOfOrderDetail = new List<OrderDetail>();

            List<Product> listOfProduct = RandomProduct(numOfOrderDetailToGenerate);

            for (int i = 0; i < numOfOrderDetailToGenerate; i++)
            {
                Random random = new Random();
                int productQuantity = i + 1;
                
                OrderDetail orderDetail = new OrderDetail
                {
                    Quantity = productQuantity,
                    Product = listOfProduct[i],
                    PurchasePrice = listOfProduct[i].Price,
                    ActivationCodes = RandomActivationCode(productQuantity, listOfProduct[i])
                };
                listOfOrderDetail.Add(orderDetail);

            }

            return listOfOrderDetail;
        }

        private List<Product> RandomProduct(int productQuantity)
        {
            List<Product> dbProductList = dbContext.Products.ToList();

            List<Product> listOfProduct = new List<Product>();

            Random random = new Random();
            int rnd = random.Next(0, dbProductList.Count - productQuantity);
            for (int j = 0; j < productQuantity; j++)
            {
                Product product = dbProductList[rnd + j];
                listOfProduct.Add(product);

            }

            return listOfProduct;
        }

        private List<ActivationCode> RandomActivationCode(int productQuantity, Product product)
        {
            List<ActivationCode> listOfActivationCode = new List<ActivationCode>();

            for (int i = 0; i < productQuantity; i++)
            {
                ActivationCode activationCode = new ActivationCode
                {
                    ProductId = product.Id
                };
                listOfActivationCode.Add(activationCode);
            }

            return listOfActivationCode;
        }
    }
}
