using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uber.Models
{
    public class Product
    {
        public string product_id { get; set; }
        public string description { get; set; }
        public string display_name { get; set; }
        public int capacity { get; set; }
        public string image { get; set; }
    }
}
