using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;

namespace ShoppingCartCA.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext dbContext;

        public CartController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            ViewData["allcartitem"] = customer.CartDetails;

            ViewData["layoutheader"] = new LayoutHeader(customer, new string[] { "Continue Shopping", "Checkout" }, false);
            return View();
        }

        public IActionResult AddToCart(string productId, int qty=1)
        {
            //To autenticate the session
            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");

            //Check if there is already this product
            CartDetail cartDetail = customer.CartDetails.FirstOrDefault(x => x.Product.Id == Guid.Parse(productId));

            if(cartDetail == null)
            {
                Product newProduct = dbContext.Products.FirstOrDefault(x => x.Id == Guid.Parse(productId));
                if(newProduct == null)
                {
                    return Json(new { updateSuccess = false });
                }
                //To create new cartdetail and add product in.
                customer.CartDetails.Add(new CartDetail()
                {
                    Product = newProduct,
                    Quantity = 1
                });
            }
            else
            {
                cartDetail.Quantity++;
            }
            dbContext.SaveChanges();

            //Return if successful and the new cart quantity
            return Json(new { updateSuccess = true, cartqty= customer.CartDetails.Count });
        }

        public IActionResult UpdateCartItem([FromBody] CartUpdateData cartUpdateData )
        {

            Customer customer = dbContext.Customers.FirstOrDefault(x => x.CustomerDetails.Username == "jeamsee");
            //CartDetail cartDetail = dbContext.CartDetails.FirstOrDefault(x => x.Id == Guid.Parse(cartUpdateData.CartItemId));
            bool cartNotUpdated = true;
            double totalcost = 0; double detailCost = 0;
            Guid cartIdToUpdate = Guid.Parse(cartUpdateData.CartItemId);

            if(cartUpdateData.Newqty <= 0)
            {
                CartDetail cartDetailDelete = customer.CartDetails.FirstOrDefault(x => x.Id == cartIdToUpdate);
                if(cartDetailDelete == null)
                {
                    return Json(new { updateSuccess = false });
                }
                customer.CartDetails.Remove(cartDetailDelete);
               cartNotUpdated = false;
            }
            
            foreach (CartDetail cartDetail in customer.CartDetails)
            {
                if(cartNotUpdated && cartDetail.Id == cartIdToUpdate)
                {
                        cartDetail.Quantity = cartUpdateData.Newqty;
                    cartNotUpdated = false;
                        detailCost = cartDetail.Quantity * cartDetail.Product.Price;
                }
                totalcost += cartDetail.Quantity * cartDetail.Product.Price;
            }
           
            //cartDetail.Quantity = cartUpdateData.Newqty;

            if (cartNotUpdated)
            { return Json(new { updateSuccess = false }); }
            dbContext.SaveChanges();

            if (cartUpdateData.Newqty > 0)
            {
                //If the row is not deleted
                return Json(new
                {
                    updateSuccess = true,
                    updateRow = cartUpdateData.UpdateRow,
                    newqty = cartUpdateData.Newqty,
                    price = String.Format("{0:0.00}", detailCost),
                    totalprice = String.Format("{0:0.00}", totalcost)
                });
            }
            return Json(new
            {
                updateSuccess = true,
                updateRow = cartUpdateData.UpdateRow,
                removeItem = true,
                totalprice = String.Format("{0:0.00}", totalcost)
            });
        }

        public IActionResult Testing()
        {
            ViewData["cartdata"] = dbContext.Products.FirstOrDefault(x => x.Name == ".NET Charts");
            return View("template");
        }
    }
}
