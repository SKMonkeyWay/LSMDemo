using API.Models;

namespace API.services
{
    public interface ISearchService
    {
        RootViewModel searchItem(string SearchQuery);
    }
}