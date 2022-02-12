using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
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
            searchFields.Add("buildings.name",9);
            searchFields.Add("buildings.shortcut",7);
            searchFields.Add("buildings.description",5);
            searchFields.Add("locks.buildingid",8);
            searchFields.Add("locks.type",3);
            searchFields.Add("locks.name",10);
            searchFields.Add("locks.serialNumber",8);
            searchFields.Add("locks.floor",6);
            searchFields.Add("locks.roomnumber",6);
            searchFields.Add("locks.description",6);
            searchFields.Add("groups.name",9);
            searchFields.Add("groups.description",5);
            searchFields.Add("medias.groupid",8);
            searchFields.Add("medias.type",3);
            searchFields.Add("medias.owner",10);
            searchFields.Add("medias.serialnumber",8);
            searchFields.Add("medias.description",6);
    
            return searchFields;
        }

        public Root search()
        {
            var webClient = new WebClient();
            var json = webClient.DownloadString(@"C:\kush\LSMDemo\API\Data\data.json");
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Root>(json);
            return data;
        }

        public Buildings searchItem(string searchTerm)
        {
            var webClient = new WebClient();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\data.json");
            string json = File.ReadAllText(path);
            Dictionary<string, object> jsonDict = new Dictionary<string, object>();
            RootViewModel  viewModel = new RootViewModel();
            viewModel =Newtonsoft.Json.JsonConvert.DeserializeObject<RootViewModel>(json);
            Dictionary<string, int> corpusDict = new Dictionary<string, int>();
            corpusDict = getCorpus();
            jsonDict = DeserializeAndFlatten(json);
            var id = "";
            foreach (var single in jsonDict)
            {
                
                var  arr = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                if (arr.Contains("id"))
                {
                    id = single.Value.ToString();
                }
                if (single.Value?.ToString().ToLower().Contains(searchTerm.ToLower()) ?? false)
                {
                    foreach (var corpus in corpusDict)
                    {
                        string[] splitedSinggles = single.Key.Split(new string[]{"."}, StringSplitOptions.None);
                        string entityName = splitedSinggles[0];
                        string comparableSingle = splitedSinggles[0] + "." + splitedSinggles[2];
                        if (comparableSingle == corpus.Key.ToLower())
                        {
                            switch(entityName)
                            {
                                case "buildings":
                                    var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                    buildings.weight += corpus.Value;

                                    var locks = viewModel.locks.Where(x => x.buildingId == id).ToList();
                                    foreach(var l in locks)
                                    {
                                        l.weight += 8;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            return null;
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