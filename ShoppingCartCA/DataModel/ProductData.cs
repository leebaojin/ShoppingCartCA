using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartCA.Models;

namespace ShoppingCartCA.DataModel
{
    public class ProductData
    {
        public static List<Product> GetSimilar(Product product)
        {
            List<Product> similarPrd = product.SimilarProducts.ToList();
            List<Product> refPrd = product.ReferenceBy.ToList();
            foreach(Product rp in refPrd)
            {
                similarPrd.Add(rp);
            }

            return similarPrd;
        }
    }
}
