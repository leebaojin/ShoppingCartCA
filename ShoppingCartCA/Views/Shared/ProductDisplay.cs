using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.Views.Shared
{
    public class ProductDisplay
    {
        public static string DisplayItem(Product product,string lastline = null)
        {
            string output = "<div class='prod-holder-table'>\n";

            //add the product image
            output += "<div class='prod-holder-row'>\n<div class='prod-col-img'>\n" +
                "<img class='prod-img'src='" + product.Img+ "' />\n</div>\n</div>";

            //add the product name
            output += "<div class='prod-holder-row'>\n<div class='prod-col-title'>\n" +
                product.Name + "</div>\n</div>";

            //add the product 
            output += "<div class='prod-holder-row'>\n<div class='prod-col-dec'>\n" +
                product.Desc + "</div>\n</div>";

            if(lastline == "purchase")
            {
                //add the button
                output += "<div class='prod-holder-row'>\n<div class='prod-col-btn'>\n" +
                    "<input class='prod-button' type='button' action='/' value='$" +
                    product.Price + " - Add to Cart' /></div>\n</div>";
            }

            return output;
        }
    }
}
