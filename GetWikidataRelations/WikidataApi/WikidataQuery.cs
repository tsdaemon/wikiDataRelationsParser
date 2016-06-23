using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace GetWikidataRelations.WikidataApi
{
    public class WikidataQuery
    {
        const string WD_ENDPOINT = "https://query.wikidata.org/sparql?format=json&query=";

        public Wikidata Get(string query)
        {
            try
            {
                query = Uri.EscapeDataString(query);
                var url = WD_ENDPOINT + query;
                var request = WebRequest.CreateHttp(url);
                using (var reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.UTF8))
                {
                    var result = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<Wikidata>(result);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
