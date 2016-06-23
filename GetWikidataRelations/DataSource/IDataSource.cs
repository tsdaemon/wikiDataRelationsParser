using System.Collections.Generic;
using GetWikidataRelations.Model;
using GetWikidataRelations.WikidataApi;

namespace GetWikidataRelations.DataSource
{
    public interface IDataSource
    {
        void SaveTriplets(IEnumerable<Triplet> values);

        List<UnitOfWork> GetWork(string id);
        void SaveWork(IEnumerable<UnitOfWork> config);

        List<Property> GetProperties(string id);
        List<Category> GetCategories(string id);
    }
}
