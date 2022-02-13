using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using API.DTOs;
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

        [HttpPost()]
        public ActionResult<RootViewModel>searchLocks(SearchDTO SearchDto)
        {
            RootViewModel viewModel = new RootViewModel();            
            viewModel = searchService.searchItem(SearchDto);            
            return viewModel;
        }
    }
}