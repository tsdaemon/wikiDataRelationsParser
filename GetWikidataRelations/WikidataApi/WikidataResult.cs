using System.Collections.Generic;
using System.Linq;

namespace GetWikidataRelations.WikidataApi
{
    public class Wikidata
    {
        public WikidataHead Head { get; set; }
        public WikidataResult Results { get; set; }
    }

    public class WikidataHead
    {
        public string[] Vars { get; set; }
    }

    public class WikidataResult
    {
        public Dictionary<string,WikidataObject>[] Bindings { get; set; }
    }

    public class WikidataObject
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public string GetIdValue()
        {
            switch (Type)
            {
                case "uri":
                    return Value.Split('/').Last();
                default:
                    return Value;
            }
        }
    }
}
