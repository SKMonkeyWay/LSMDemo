using API.Models;
using API.services;
using API.Utility;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    [MemoryDiagnoser]
    public class SearchBenchmark
    {
        private readonly string _json = Helper.GetSerializedJSON();

        [Benchmark(Baseline = true)]
        public void Old()
        {
            var root = JsonConvert.DeserializeObject<RootViewModel>(_json)!;
            var service = new SearchService(_json, root);
            service.searchItem("uid");
        }

        [Benchmark]
        public void New()
        {
            var root = JsonConvert.DeserializeObject<RootViewModel>(_json)!;
            var service = new ObjectSearch(root);
            service.searchItem("uid");
        }
    }
}
