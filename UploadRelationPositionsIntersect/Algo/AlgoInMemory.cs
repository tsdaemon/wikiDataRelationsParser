using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.Service;
using Core.Wikifile;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect.Algo
{
    public class AlgoInMemory : AlgoBase
    {
        private Dictionary<string, TripletMini[]> _tripletsInMemory;

        public AlgoInMemory(AsyncSaver saver, IWikidumpReader wikiReader, IMongoCollection<Triplet> triplets, string positionsPath) 
            : base(saver, wikiReader, triplets, positionsPath)
        {
            _tripletsInMemory = GetTripletsCache();
        }

        protected override int ProcessPair(PositionLine entity1, PositionLine entity2)
        {
            var p = 0;
            TripletMini[] array;

            if (_tripletsInMemory.TryGetValue(entity1.EntityName + entity2.EntityName, out array))
            {
                foreach (var t in array)
                {
                    ProcessTriplet(t.Id, entity1, entity2, _wikiReader);
                    p++;
                }
            }

            if (_tripletsInMemory.TryGetValue(entity2.EntityName + entity1.EntityName, out array))
            {
                foreach (var t in array)
                {
                    ProcessTriplet(t.Id, entity2, entity1, _wikiReader);
                    p++;
                }
            }
            return p;
        }

        private Dictionary<string, TripletMini[]> GetTripletsCache()
        {
            var fields = Builders<Triplet>.Projection
                .Include(t => t.Id)
                .Include(t => t.ObjectWikiName)
                .Include(t => t.SubjectWikiName);
            return _triplets
                .Find(r => true)
                .Project(fields)
                .As<TripletMini>()
                .ToEnumerable()
                .GroupBy(t => t.ObjectWikiName + t.SubjectWikiName)
                .ToDictionary(t => t.Key, t => t.ToArray());
        }
    }
}
