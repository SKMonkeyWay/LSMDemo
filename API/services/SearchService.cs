using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using API.DTOs;
using API.Models;
using API.Helper;
using Newtonsoft.Json.Linq;

namespace API.services
{
    public class SearchService : ISearchService
    {
        IServiceProvider service = null;
        public SearchService(IServiceProvider _service)
        {
            service = _service;
        }

        public static Dictionary<string, int> getCorpus()
        {
            Dictionary<string, int> searchFields = new Dictionary<string, int>();
            searchFields.Add("buildings.name", 9);
            searchFields.Add("buildings.shortcut", 7);
            searchFields.Add("buildings.description", 5);
            searchFields.Add("locks.buildingid", 8);
            searchFields.Add("locks.type", 3);
            searchFields.Add("locks.name", 10);
            searchFields.Add("locks.serialNumber", 8);
            searchFields.Add("locks.floor", 6);
            searchFields.Add("locks.roomnumber", 6);
            searchFields.Add("locks.description", 6);
            searchFields.Add("groups.name", 9);
            searchFields.Add("groups.description", 5);
            searchFields.Add("medias.groupid", 8);
            searchFields.Add("medias.type", 3);
            searchFields.Add("medias.owner", 10);
            searchFields.Add("medias.serialnumber", 8);
            searchFields.Add("medias.description", 6);

            return searchFields;
        }

        public RootViewModel searchItem(SearchDTO SearchDto)
        {
            string json = GetSerializedJSON();
            RootViewModel viewModel = new RootViewModel();
            viewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RootViewModel>(json);
            viewModel = WeightCalcution(SearchDto, viewModel, json);
            return viewModel;
        }

        public static RootViewModel WeightCalcution(SearchDTO SearchDto, RootViewModel viewModel, string json)
        {
            //string json = GetSerializedJSON();
            Dictionary<string, object> jsonDict = new Dictionary<string, object>();

            Dictionary<string, int> corpusDict = new Dictionary<string, int>();
            corpusDict = getCorpus();
            jsonDict = DeserializeAndFlatten(json);
            var id = "";

            foreach (var single in jsonDict)
            {

                if (single.Value?.ToString().ToLower() == SearchDto.SearchTerm.ToLower())
                {
                    var arr = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                    if (arr.Contains("id"))
                    {
                        id = single.Value.ToString();
                    }
                    foreach (var corpus in corpusDict)
                    {
                        string[] splitedSinggles = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                        string entityName = splitedSinggles[0];
                        string comparableSingle = splitedSinggles[0] + "." + splitedSinggles[2];
                        if (comparableSingle.ToLower() == corpus.Key.ToLower())
                        {
                            switch (entityName)
                            {

                                // when the search query is found on the different fields of buildings. 
                                case "buildings":
                                    if (comparableSingle == "buildings.name")
                                    {
                                        var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                        buildings.weight += corpus.Value * 10;

                                        //For transferring the transitive weights to the locks of this buildings. 
                                        var locks = viewModel.locks.Where(x => x.buildingId == id).ToList();
                                        foreach (var l in locks)
                                        {
                                            l.weight += (8) * 10; //transitive weight of name and shortcut from buildings model.
                                        }
                                    }
                                    if (comparableSingle == "buildings.shortcut")
                                    {
                                        var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                        buildings.weight += 7 * 10;
                                    }
                                    if (comparableSingle == "buildings.description")
                                    {
                                        var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                        buildings.weight += 5 * 10;
                                    }

                                    break;


                                // when the search query is found on the different fields of locks
                                case "locks":
                                    if (comparableSingle == "locks.name")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += corpus.Value * 10;
                                    }
                                    if (comparableSingle == "locks.type")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 3 * 10;
                                    }
                                    if (comparableSingle == "locks.serialnumber")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 8 * 10;
                                    }
                                    if (comparableSingle == "locks.floor")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * 10;
                                    }
                                    if (comparableSingle == "locks.roomnumber")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * 10;
                                    }
                                    if (comparableSingle == "locks.description")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * 10;
                                    }
                                    break;


                                case "groups":
                                    if (comparableSingle == "groups.name")
                                    {
                                        var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                        groups.weight += corpus.Value * 10;
                                    }

                                    if (comparableSingle == "groups.description")
                                    {
                                        var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                        groups.weight += 5 * 10;
                                    }

                                    var medias = viewModel.media.Where(x => x.groupId == id).ToList();
                                    foreach (var media in medias)
                                    {
                                        media.weight += 8 * 10;
                                    }
                                    break;

                                case "media":
                                    if (comparableSingle == "media.name")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += corpus.Value * 10;
                                    }
                                    if (comparableSingle == "media.type")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 3 * 10;
                                    }
                                    if (comparableSingle == "media.owner")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 10 * 10;
                                    }
                                    if (comparableSingle == "media.serialnumber")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 8 * 10;
                                    }
                                    if (comparableSingle == "media.description")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 6 * 10;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    return viewModel;
                }
                else
                {
                    var arr = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                    if (arr.Contains("id"))
                    {
                        id = single.Value.ToString();
                    }
                    string[] searchTerms = SearchDto.SearchTerm.Split(new string[] { " " }, StringSplitOptions.None);
                    foreach (var searchTerm in searchTerms)
                    {
                        if (single.Value?.ToString().ToLower().Contains(searchTerm.ToLower()) ?? false)
                        {
                            foreach (var corpus in corpusDict)
                            {
                                string[] splitedSinggles = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                                string entityName = splitedSinggles[0];
                                string comparableSingle = splitedSinggles[0] + "." + splitedSinggles[2];
                                if (comparableSingle.ToLower() == corpus.Key.ToLower())
                                {
                                    switch (entityName)
                                    {

                                        // when the search query is found on the different fields of buildings. 
                                        case "buildings":
                                            if (comparableSingle == "buildings.name")
                                            {
                                                var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                                buildings.weight += corpus.Value;

                                                //For transferring the transitive weights to the locks of this buildings. 
                                                var locks = viewModel.locks.Where(x => x.buildingId == id).ToList();
                                                foreach (var l in locks)
                                                {
                                                    l.weight += 8; //transitive weight of name and shortcut from buildings model.
                                                }
                                            }
                                            if (comparableSingle == "buildings.shortcut")
                                            {
                                                var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                                buildings.weight += 7;
                                            }
                                            if (comparableSingle == "buildings.description")
                                            {
                                                var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                                buildings.weight += 5;
                                            }

                                            break;


                                        // when the search query is found on the different fields of locks
                                        case "locks":
                                            if (comparableSingle == "locks.name")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += corpus.Value;
                                            }
                                            if (comparableSingle == "locks.type")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += 3;
                                            }
                                            if (comparableSingle == "locks.serialnumber")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += 8;
                                            }
                                            if (comparableSingle == "locks.floor")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += 6;
                                            }
                                            if (comparableSingle == "locks.roomnumber")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += 6;
                                            }
                                            if (comparableSingle == "locks.description")
                                            {
                                                var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                                lks.weight += 6;
                                            }
                                            break;


                                        case "groups":
                                            if (comparableSingle == "groups.name")
                                            {
                                                var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                                groups.weight += corpus.Value;
                                            }

                                            if (comparableSingle == "groups.description")
                                            {
                                                var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                                groups.weight += 5;
                                            }

                                            var medias = viewModel.media.Where(x => x.groupId == id).ToList();
                                            foreach (var media in medias)
                                            {
                                                media.weight += 8;
                                            }
                                            break;

                                        case "media":
                                            if (comparableSingle == "media.name")
                                            {
                                                var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                                mds.weight += corpus.Value;
                                            }
                                            if (comparableSingle == "media.type")
                                            {
                                                var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                                mds.weight += 3;
                                            }
                                            if (comparableSingle == "media.owner")
                                            {
                                                var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                                mds.weight += 10;
                                            }
                                            if (comparableSingle == "media.serialnumber")
                                            {
                                                var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                                mds.weight += 8;
                                            }
                                            if (comparableSingle == "media.description")
                                            {
                                                var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                                mds.weight += 6;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }

                    }

                }
            }
            return viewModel;
        }




        public static string GetSerializedJSON()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\data.json");
            string json = File.ReadAllText(path);
            return json;
        }

        public static Dictionary<string, object> DeserializeAndFlatten(string json)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            JToken token = JToken.Parse(json);
            FillDictionaryFromJToken(dict, token, "");
            return dict;
        }

        private static void FillDictionaryFromJToken(Dictionary<string, object> dict, JToken token, string prefix)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty prop in token.Children<JProperty>())
                    {
                        FillDictionaryFromJToken(dict, prop.Value, Join(prefix, prop.Name));
                    }
                    break;

                case JTokenType.Array:
                    int index = 0;
                    foreach (JToken value in token.Children())
                    {
                        FillDictionaryFromJToken(dict, value, Join(prefix, index.ToString()));
                        index++;
                    }
                    break;

                default:
                    dict.Add(prefix, ((JValue)token).Value);
                    break;
            }
        }

        private static string Join(string prefix, string name)
        {
            return (string.IsNullOrEmpty(prefix) ? name : prefix + "." + name);
        }
    }
}