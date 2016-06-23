using System.Collections.Generic;
using System.Linq;
using GetWikidataRelations.Model;
using GetWikidataRelations.WikidataApi;
using MongoDB.Driver;

namespace GetWikidataRelations.DataSource
{
    public class MongoDataSource : IDataSource
    {
        private IMongoDatabase _data;
        private IMongoCollection<Property> _properties;
        private IMongoCollection<Category> _categories;
        private IMongoCollection<Triplet> _triplets;
        private IMongoCollection<UnitOfWork> _units;

        public MongoDataSource(string url, string database)
        {
            _data = new MongoClient(url).GetDatabase(database);
            _triplets = _data.GetCollection<Triplet>("triplet");
            _units = _data.GetCollection<UnitOfWork>("unit");
            _properties = _data.GetCollection<Property>("property");
            _categories = _data.GetCollection<Category>("category");
        }

        public void SaveTriplets(IEnumerable<Triplet> documents)
        {
            _triplets.InsertManyAsync(documents);
        }

        public List<UnitOfWork> GetWork(string id)
        {
            return _units.Find(u => u.SectionId == id).ToList();
        }

        public void SaveWork(IEnumerable<UnitOfWork> units)
        {
            foreach (var unit in units)
            {
                var unit1 = unit;
                _units.FindOneAndReplaceAsync(u => u.Id == unit1.Id, unit);
            }
        }

        public List<Property> GetProperties(string id)
        {
            return _properties.Find(u => u.SectionId == id).ToList();
        }

        public List<Category> GetCategories(string id)
        {
            return _categories.Find(u => u.SectionId == id).ToList();
        }
    }
}
