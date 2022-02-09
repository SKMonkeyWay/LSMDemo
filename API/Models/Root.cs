using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Root
    {
        public List<Buildings> buildings { get; set; }
        public List<Locks> locks { get; set; }
        public List<Groups> groups { get; set; }
        public List<Media> media { get; set; }
    }
}