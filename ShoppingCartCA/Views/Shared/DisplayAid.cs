using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;
using System.Text;

namespace ShoppingCartCA.Views.Shared
{
    public class DisplayAid
    {
        public static string DisplayProduct(Product product,string lastbutton = null)
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

            if(lastbutton == "Purchase")
            {
                //add the button
                output += "<div class='prod-holder-row'>\n<div class='prod-col-btn'>\n" +
                    "<input class='prod-button' type='button' onclick='AddToCart(\"" + @product.Id + "\")' value='$" +
                    product.Price + " - Add to Cart' /></div>\n</div>";
            }else if(lastbutton == "Download")
            {
                output += "<div class='prod-holder-row'>\n<div class='prod-col-btn'>\n" +
                    "<a href='"+ product.DownloadFile + "' download='"+product.DownloadName+"'>"+
                    "<input class='prod-button' type='button' value='Download' /></div>\n</div>";
            }

            output += "</div>";

            return output;
        }

    }
}
