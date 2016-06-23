using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace GetWikidataRelations.WikidataApi
{
    public class WikidataQuery
    {
        const string WD_ENDPOINT = "https://query.wikidata.org/sparql?query={0}&format=json";

        public Wikidata Get(string query, params object[] args)
        {
            try
            {
                query = Uri.EscapeDataString(string.Format(query, args));
                var url = string.Format(WD_ENDPOINT, query);
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
