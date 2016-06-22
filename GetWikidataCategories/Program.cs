using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GetWikidataRelations.WikidataApi;

namespace GetWikidataCategories
{
    class Program
    {
        // basic classes
        static Dictionary<string, string[]> categories = new Dictionary<string, string[]>
        {
            {"../../../categories_organization.txt", new []{"Q43229"}},
            {"../../../categories_location.txt", 
                new []{
                    "Q1048835",     // political territorial entity
                    "Q15642541",    // human-geographic territorial entity
                    "Q17334923",    // location
                    "Q618123"       // geographical object
                } 
            }
        };

        private static WikidataQuery _api;

        const string WD_CAT_QUERY = @"SELECT ?s ?sLabel
WHERE {{
  ?s wdt:P279 wd:{0} .
  ?s rdfs:label ?sLabel filter(LANG(?sLabel) = 'en')
}}";

        static void Main(string[] args)
        {
            _api = new WikidataQuery();
            categories.AsParallel().ForAll(k => ReadAllCategories(k.Key, k.Value));
        }

        private static void ReadAllCategories(string file, string[] rootCategories)
        {
            var list = new Dictionary<string,string>();
            foreach (var cat in rootCategories)
            {
                list.Add(cat, "");
                GetAllSubclasses(cat, list);
            }
            using (var wr = new StreamWriter(File.OpenWrite(file)))
            {
                foreach (var cat in list)
                {
                    wr.WriteLine("{0} {1}", cat.Key, cat.Value);
                }
            }
        }

        private static void GetAllSubclasses(string cat, Dictionary<string, string> list)
        {
            var result = _api.Get(WD_CAT_QUERY, cat);
            foreach (var r in result.Results.Bindings)
            {
                var id = r["s"].GetIdValue();
                var label = r["sLabel"].Value;
                if (!list.ContainsKey(id))
                {
                    list.Add(id, label);
                    GetAllSubclasses(id, list);
                }
            }
        }
    }
}
