using System.Collections.Generic;
using System.IO;
using System.Reflection;
using API.Enums;
using Newtonsoft.Json.Linq;

namespace API.Utility
{
    public static class Helper
    {
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
            searchFields.Add("media.groupid", 8);
            searchFields.Add("media.type", 3);
            searchFields.Add("media.owner", 10);
            searchFields.Add("media.serialnumber", 8);
            searchFields.Add("media.description", 6);

            return searchFields;
        }

        public static SearchTypeEnum GetSearchType(string SearchText, Dictionary<string, object> SearchedList)
        {
            return SearchTypeEnum.ExactSearch;

        }
    }
}