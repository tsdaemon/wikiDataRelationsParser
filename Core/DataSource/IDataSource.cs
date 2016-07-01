using System.Collections.Generic;
using Core.Model;

namespace Core.DataSource
{
    public interface IDataSource
    {
        void SaveTriplets(IEnumerable<Triplet> values);

        Unit GetWork(string id);
        void SaveWork(Unit work);

        List<Property> GetProperties(string id);
        List<Category> GetCategories(string id);
    }
}
