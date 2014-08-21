namespace Uber.Models
{
    public class History
    {
        public string uuid { get; set; }
        public int request_time { get; set; }
        public string product_id { get; set; }
        public string status { get; set; }
        public double distance { get; set; }
        public int start_time { get; set; }
        public Location start_location { get; set; }
        public int end_time { get; set; }
        public Location end_location { get; set; }
    }
}