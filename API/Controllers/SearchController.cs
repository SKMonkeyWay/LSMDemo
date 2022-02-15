using API.DTOs;
using API.Models;
using API.services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public IActionResult searchLocks(SearchQueryDto searchqueryDto)
        {
            string msg = "";
            if (string.IsNullOrEmpty(searchqueryDto.searchQuery)){
                msg = "please search something....";
                return Ok(msg);
            }
            else
            {
                RootViewModel viewModel = searchService.searchItem(searchqueryDto.searchQuery);
                return Ok(viewModel);
            }        
            
        }
    }
}