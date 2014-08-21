using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uber.Models
{
    public class ResponseHeader
    {
        public string RateLimitRemaining { get; set; }
        public string Etag { get; set; }
        public string RateLimitReset { get; set; }
        public string RateLimitLimit { get; set; }
        public string UberApp { get; set; }
    }
}
