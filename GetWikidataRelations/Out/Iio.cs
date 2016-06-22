using System.Collections.Generic;
using GetWikidataRelations.WikidataApi;
using GetWikidataRelations.Worker;

namespace GetWikidataRelations.Out
{
    public interface Iio
    {
        void WriteProperty(Dictionary<string, WikidataObject> result, string property);
        WorkersPoolInfo GetInfo(string id);
        void SaveInfo(string id, WorkersPoolInfo config);
    }
}
