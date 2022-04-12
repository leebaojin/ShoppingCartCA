using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartCA.Models
{
    public class DataReviewPost
    {
        public int ReviewRating { get; set; }

        public string ReviewMsg { get; set; }

        public string PrdId { get; set; }

        public string CommentId { get; set; }
    }
}
