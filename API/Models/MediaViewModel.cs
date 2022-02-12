using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class MediaViewModel
    {
        public string id { get; set; }
        public string groupId { get; set; }
        public string type { get; set; }
        public string owner { get; set; }
        public object description { get; set; }
        public string serialNumber { get; set; }
        public int weight { get; set; }
    }
}