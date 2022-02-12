using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.Models;
using API.services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService searchService = null;
        public SearchController(ISearchService searchService)
        {
            this.searchService = searchService;
        }

        [HttpGet]
        public Root getData(){
            var data = searchService.search();
            return data;
        }

        [HttpGet("{searchTerm}")]
        public Buildings getData(string searchTerm){
            var data = searchService.searchItem(searchTerm);
            return data;
        }
    }
}