using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Models;

namespace API.services
{
    public class SearchService : ISearchService
    {
        IServiceProvider service = null;
        public SearchService(IServiceProvider _service)
        {
            service = _service;
        }

        public Root search()
        {
           var webClient = new WebClient();
            var json = webClient.DownloadString(@"C:\kush\LSMDemo\API\Data\data.json");
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(json);
            return data;
        }
    }
}