using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API.services
{
    public interface ISearchService
    {
        Root search();
    }
}