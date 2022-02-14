using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class LocksViewModel
    {
        public string id { get; set; }
        public string buildingId { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string serialNumber { get; set; }
        public string floor { get; set; }
        public string roomNumber { get; set; }
        public int weight { get; set; }
    }
}