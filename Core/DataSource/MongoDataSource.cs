using System.Collections.Generic;
using Core.Model;
using MongoDB.Driver;

namespace Core.DataSource
{
    public class MongoDataSource : IDataSource
    {
        private IMongoDatabase _data;
        private IMongoCollection<Property> _properties;
        private IMongoCollection<Category> _categories;
        private IMongoCollection<Triplet> _triplets;
        private IMongoCollection<Unit> _units;

        public MongoDataSource(string host, int port, string database)
        {
            _data = new MongoClient(new MongoClientSettings { UseSsl = false, Server = new MongoServerAddress(host, port) }).GetDatabase(database);
            _triplets = _data.GetCollection<Triplet>("triplet");
            _units = _data.GetCollection<Unit>("unit");
            _properties = _data.GetCollection<Property>("property");
            _categories = _data.GetCollection<Category>("category");
        }

        public void SaveTriplets(IEnumerable<Triplet> documents)
        {
            try
            {
                _triplets.InsertManyAsync(documents);
            }
            catch
            {
                
            }
        }

        public Unit GetWork(string id)
        {
            return _units.Find(u => u.Id == id).FirstOrDefault();
        }

        public void SaveWork(Unit work)
        {
            try
            {
                if (_units.Find(f => f.Id == work.Id).Any())
                {
                    _units.ReplaceOne(f => f.Id == work.Id, work);
                }
                else
                {
                    _units.InsertOne(work);
                }
            }
            catch
            {
                
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
