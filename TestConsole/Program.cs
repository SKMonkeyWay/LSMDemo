using API.Models;
using API.services;
using API.Utility;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;
using TestConsole;

//BenchmarkRunner.Run<SearchBenchmark>();


//var query = "Produktionsstätte";
var query = "hoff";
//PrintNew(query);
//PrintOld(query);

var newRoot = SearchNew(query);
var oldRoot = SearchOld(query);

Console.WriteLine($"Old: count: {GetRootNodeCount(oldRoot)}, weight: {GetRootWeight(oldRoot)}");
PrintRoot(oldRoot);
Console.WriteLine($"New: count: {GetRootNodeCount(newRoot)}, weight: {GetRootWeight(newRoot)}");
PrintRoot(newRoot);

RootViewModel SearchNew(string query)
{
    var json = Helper.GetSerializedJSON();
    var root = JsonConvert.DeserializeObject<RootViewModel>(json);
    var search = new ObjectSearch(root);
    root = search.searchItem(query);
    return root;
}

RootViewModel SearchOld(string query)
{
    var json = Helper.GetSerializedJSON();
    var root = JsonConvert.DeserializeObject<RootViewModel>(json);
    var service = new SearchService(json,root);
    return service.searchItem(query);
}

void PrintRoot(RootViewModel root)
{
    foreach (var building in root.buildings)
        Console.WriteLine($"{building.name}, {building.weight}");
    foreach (var @lock in root.locks)
        Console.WriteLine($"{@lock.name}, {@lock.weight}");
    foreach (var group in root.groups)
        Console.WriteLine($"{group.name}, {group.weight}");
    foreach (var media in root.media)
        Console.WriteLine($"{media.serialNumber}, {media.weight}");
}

void PrintNew(string query)
{
    var json = Helper.GetSerializedJSON();
    var root = JsonConvert.DeserializeObject<RootViewModel>(json);
    var search = new LockSearch();
    var context = new RootSearchContext { Root = root };
    foreach (var @lock in root.locks)
    {
        search.Search(context, @lock, query);
    }
    Console.WriteLine("=== NEW =======================================");
    foreach (var @lock in root.locks.Where(b => b.weight > 0))
    {
        Console.WriteLine($"{@lock.name}, {@lock.weight}");
    }
}

void PrintOld(string query)
{
    var json = Helper.GetSerializedJSON();
    var root = JsonConvert.DeserializeObject<RootViewModel>(json);
    var service = new SearchService(json, root);
    var ret = service.searchItem(query);

    Console.WriteLine("=== OLD =======================================");
    foreach (var @lock in ret.locks)
    {
        Console.WriteLine($"{@lock.name}, {@lock.weight}");
    }
}

int GetRootWeight(RootViewModel root)
{
    return
        root.buildings.Sum(b => b.weight) +
        root.locks.Sum(l => l.weight) +
        root.groups.Sum(g => g.weight) +
        root.media.Sum(m => m.weight);
}

int GetRootNodeCount(RootViewModel root)
{
    return
        root.buildings.Count +
        root.locks.Count +
        root.groups.Count +
        root.media.Count;
}