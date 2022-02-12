using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class BuildingsViewModel
    {
        public string id { get; set; }
        public string shortCut { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int weight { get; set; }
    }
}