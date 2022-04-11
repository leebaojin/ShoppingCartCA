using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.DataModel
{
    public class PathData
    {
        public static string GetDownloadLink(string filename)
        {
            return "../Download/" + filename;
        }

        public static string GetImgLink(string filename)
        {
            return "../pictures/" + filename;
        }
    }
}
