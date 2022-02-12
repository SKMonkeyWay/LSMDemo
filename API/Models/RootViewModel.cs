using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class RootViewModel
    {

        public RootViewModel()
        {
            buildings = new List<BuildingsViewModel>();
            locks = new List<LocksViewModel>();
            groups = new List<GroupsViewModel>();
            media = new List<MediaViewModel>();
        }

        public List<BuildingsViewModel> buildings { get; set; }
        public List<LocksViewModel> locks { get; set; }
        public List<GroupsViewModel> groups { get; set; }
        public List<MediaViewModel> media { get; set; }
    }
}