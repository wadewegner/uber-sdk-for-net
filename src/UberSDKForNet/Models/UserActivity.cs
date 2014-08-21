using System.Collections.Generic;

namespace Uber.Models
{
    public class UserActivity
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int count { get; set; }
        public List<History> history { get; set; }
    }
}