using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using API.Utility;
using API.Enums;

namespace API.services
{
    public class SearchService : ISearchService
    {
        private readonly string _json;
        private readonly RootViewModel _root;

        public SearchService(string json, RootViewModel root)
        {
            _json = json;
            _root = root;
        }

        public RootViewModel searchItem(string SearchQuery)
        {
            //string jsonData = Helper.GetSerializedJSON();
            //RootViewModel viewModel = new RootViewModel();
            //viewModel = Newtonsoft.Json.JsonConvert.DeserializeObject<RootViewModel>(jsonData);
            var viewModel = WeightCalcution(SearchQuery, _root, _json);
            return new RootViewModel
            {
                buildings = viewModel.buildings.Where(x => x.weight > 0).OrderByDescending(x => x.weight).ToList(),
                locks = viewModel.locks.Where(x => x.weight > 0).OrderByDescending(x => x.weight).ToList(),
                groups = viewModel.groups.Where(x => x.weight > 0).OrderByDescending(x => x.weight).ToList(),
                media = viewModel.media.Where(x => x.weight > 0).OrderByDescending(x => x.weight).ToList()
            };

        }

        // For calcuating the weight of the entity against the searchterm in two conditions:(1. For exact match  2. For partial match)
        public static RootViewModel WeightCalcution(string SearchQuery, RootViewModel viewModel, string jsonData)
        {
            Dictionary<string, object> dataDictionary = new Dictionary<string, object>();
            dataDictionary = Helper.DeserializeAndFlatten(jsonData);

            bool exactMatch = false;
            foreach (var item in dataDictionary)
            {
                if (item.Value?.ToString().ToLower() == SearchQuery.ToLower().Trim())
                    exactMatch = true;
            }

            if (exactMatch)
            {
                viewModel = assignWeight(dataDictionary, viewModel, SearchTypeEnum.ExactSearch, SearchQuery);
            }
            else
            {
                string[] searchTerms = SearchQuery.Split(new string[] { " " }, StringSplitOptions.None);
                foreach (var searchTerm in searchTerms)
                {
                    viewModel = assignWeight(dataDictionary, viewModel, SearchTypeEnum.PartialSearch, searchTerm);
                }
            }
            return viewModel;
        }


        public static RootViewModel assignWeight(Dictionary<string, object> jsonDict, RootViewModel viewModel, SearchTypeEnum searchType, string SearchTerm)
        {
            var id = "";
            // acquaring the dictionary with fields name and their respective weights from helper function. 
            Dictionary<string, int> corpusDict = Helper.getCorpus();
            
            //synchronising data dictionary with fields dictionary to match with the search term and give weights to the fields
            foreach (var single in jsonDict)
            {
                var arr = single.Key.Split(new string[] { "." }, StringSplitOptions.None);
                if (arr.Contains("id"))
                {
                    id = single.Value.ToString();
                }
                if (single.Value?.ToString().ToLower().Contains(SearchTerm.ToLower()) ?? false)
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
                                        buildings.weight += corpus.Value * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);

                                        //For transferring the transitive weights to the locks of this buildings. 
                                        var locks = viewModel.locks.Where(x => x.buildingId == id).ToList();
                                        foreach (var l in locks)
                                        {
                                            l.weight += (8) * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1); //transitive weight of name and shortcut from buildings model.
                                        }
                                    }
                                    if (comparableSingle == "buildings.shortCut")
                                    {
                                        var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                        buildings.weight += 7 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "buildings.description")
                                    {
                                        var buildings = viewModel.buildings.FirstOrDefault(x => x.id == id);
                                        buildings.weight += 5 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }

                                    break;


                                // when the search query is found on the different fields of locks
                                case "locks":
                                    if (comparableSingle == "locks.name")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += corpus.Value * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "locks.type")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 3 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "locks.serialNumber")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 8 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "locks.floor")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "locks.roomNumber")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "locks.description")
                                    {
                                        var lks = viewModel.locks.FirstOrDefault(x => x.id == id);
                                        lks.weight += 6 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    break;


                                case "groups":
                                    if (comparableSingle == "groups.name")
                                    {
                                        var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                        groups.weight += corpus.Value * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);

                                        var medias = viewModel.media.Where(x => x.groupId == id).ToList();
                                        foreach (var media in medias)
                                        {
                                            media.weight += 8 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                        }

                                    }

                                    if (comparableSingle == "groups.description")
                                    {
                                        var groups = viewModel.groups.FirstOrDefault(x => x.id == id);
                                        groups.weight += 5 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    break;

                                case "media":
                                    if (comparableSingle == "media.name")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += corpus.Value * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "media.type")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 3 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "media.owner")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 10 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "media.serialNumber")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 8 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    if (comparableSingle == "media.description")
                                    {
                                        var mds = viewModel.media.FirstOrDefault(x => x.id == id);
                                        mds.weight += 6 * (searchType == SearchTypeEnum.ExactSearch ? 10 : 1);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

            }

            return viewModel;
        }
    }
}