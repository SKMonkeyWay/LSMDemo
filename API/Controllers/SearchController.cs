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

        [HttpGet]
        public ActionResult<RootViewModel> searchLocks([FromQuery] string Searchquery)
        {
            RootViewModel viewModel = searchService.searchItem(Searchquery);
            return viewModel;
        }
    }
}