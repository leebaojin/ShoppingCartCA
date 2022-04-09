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
        public static string DisplayProduct(Product product,string lastbutton = null,string searchVal = null)
        {
            if(product == null)
            {
                return "";
            }
            string displayName = DisplaySearch(product.Name, searchVal);
            string displayDesc = DisplaySearch(product.Desc, searchVal);

            string output = "<div class='prod-holder'><div class='prod-holder-table'>\n";

            //add the product image
            output += "<div class='prod-holder-row'>\n<div class='prod-col-img'>\n" +
                "<img class='prod-img'src='" + product.Img+ "' />\n</div>\n</div>";

            //add the product name
            output += "<div class='prod-holder-row'>\n<div class='prod-col-title'>\n" +
                displayName + "</div>\n</div>";

            //Create the more link
            string morelink = " <a class='prod-morebtn' href='../Home/ProdDetail?prdId="+product.Id+"'>...more</a>";

            //add the product 
            output += "<div class='prod-holder-row'>\n<div class='prod-col-dec'>\n" +
                displayDesc + morelink + "</div>\n</div>";

            if(lastbutton == "Purchase")
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

    }
}
