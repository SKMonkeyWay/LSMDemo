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
            RootViewModel viewModel = searchService.searchItem(searchqueryDto.searchQuery);
            return Ok(viewModel);
        }
    }
}