using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;
using ShoppingCartCA.DataModel;
using System.Text;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartCA.Views.Shared
{
    public class DisplayAid
    {
        public static string DisplayProduct(Product product, bool border)
        {
            return DisplayProduct(product, null, null, border);
        }
        public static string DisplayProduct(Product product,string lastbutton = null, string searchVal = null, bool border=true)
        {
            if(product == null)
            {
                return "";
            }
            string displayName = DisplaySearch(product.Name, searchVal);
            string displayDesc = DisplaySearch(product.Desc, searchVal);
            string borderclass;
            if (border)
            {
                borderclass = "class='prod-holder'";
            }
            else
            {
                borderclass = "class='prod-holdercenter'";
            }
            string morelink = " <a href='../Home/ProdDetail?prdId=" + product.Id + "'>\n";
            //<a href='../Home/ProdDetail?prdId=@product.Id'>

            string output = "<div "+ borderclass +"><div class='prod-holder-table'>\n";

            //add more link
            output += morelink;

            //add the product image
            output += "<div class='prod-holder-row'>\n<div class='prod-col-img'>\n" +
                "<img class='prod-img'src='" + product.Img+ "' />\n" +
                "</div>\n</div>";

            //add the product name
            output += "<div class='prod-holder-row'>\n<div class='prod-col-title'>\n" +
                displayName + 
                "</div>\n</div>";
           

            //add the product 
            output += "<div class='prod-holder-row'>\n<div class='prod-col-dec'>\n" +
                displayDesc + 
                "</div>\n</div>";

            //Close link
            output += "</a>\n";

            if (lastbutton == "Purchase")
            {
                //add the button
                output += "<div class='prod-holder-row'>\n<div class='prod-col-btn'>\n" +
                    "<button class='prod-button' onclick='AddToCart(\"" + @product.Id + "\",1)'>" +
                    "$ " + product.Price+ " -  <i class='fa fa-shopping-cart'></i> Add to Cart</button>" +
                    "\n</div>\n</div>";

            }else if(lastbutton == "Download")
            {
                output += "<div class='prod-holder-row'>\n<div class='prod-col-btn'>\n" +
                    "<a href='"+ product.DownloadFile + "' download='"+product.DownloadName+"'>"+
                    "<button class='prod-button' value='Download'>" +
                    "<i class='fa fa-download'></i> Download</button>" +
                    "\n</div>\n</div>";
            }

            

            output += "\n</div>\n</div>";

            return output;
        }

        public static string DisplaySearch(string input, string searchVal)
        {
            if (searchVal == null || searchVal == "")
            {
                return input;
            }
            string highlightstart = "<span class='prod-findtxt'>";
            string highlightend = "</span>";
            string outputstring = "";
            int currpos = 0;
            int searchlen = searchVal.Length;
            string inputlower = input.ToLower();
            string searchlower = searchVal.ToLower();

            while (currpos < input.Length)
            {
                int findindex = inputlower.IndexOf(searchlower, currpos);
                if (findindex == -1)
                {
                    outputstring += input.Substring(currpos);
                    currpos = input.Length;
                    break;
                }
                else
                {
                    if (currpos != findindex)
                    {
                        outputstring += input.Substring(currpos, (findindex - currpos));
                        currpos = findindex;
                    }
                    outputstring += highlightstart + input.Substring(currpos, searchlen) + highlightend;
                    currpos += searchlen;
                }
            }

            return outputstring;
        }

        public static string DisplaySmall(Product product)
        {
            string output = "<div class='small-holder'>\n";

            //Add a link
            output += "<a href='../Home/ProdDetail?prdId=" + product.Id + "'>\n";

            output += "<div class='small-prd-tbl'>\n";

            //Add the image
            output += "<div class='small-prd-row'>\n<div class='small-col-img'>\n";
            output += "<img class='small-prod-img' src='"+ product.Img + "' />\n";
            output += "</div>\n</div>\n";

            //Add the name
            output += "<div class='small-prd-row'>\n<div class='small-col-title'>\n";
            output += product.Name;
            output += "</div>\n</div>\n";

            //Add the price
            output += "<div class='small-prd-row'>\n<div class='small-col-price'>\n";
            output += "$ " + String.Format("{0:0.00}", product.Price);
            output += "</div>\n</div>\n";

            //Close
            output += "</div>\n</a>\n</div>";

            return output;
        }

        public static string GenerateStar(int rating)
        {
            int withstar;
            int curr = 0;
            if(rating < 1 || rating > 5)
            {
                withstar = 0;
            }
            else
            {
                withstar = rating;
            }

            string output = "<span>";
            while (curr < 5)
            {
                output += "<span ";
                if (curr < withstar)
                {
                    output += "class=\"fa fa-star checked\" ";
                }
                else
                {
                    output += "class=\"fa fa-star\" ";
                }

                output += "id=\"ratestar-" + curr + "\" " +
                    "onclick=\"StarSelect(" + (curr).ToString() + ")\"></span>";
                curr++;
            }
            output += "</span>";
            return output;
        }

    }
}
