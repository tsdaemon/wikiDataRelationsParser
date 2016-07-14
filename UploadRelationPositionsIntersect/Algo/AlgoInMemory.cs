using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Model;
using Core.Service;
using Core.Wikifile;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect.Algo
{
    public class AlgoInMemory : AlgoBase
    {
        private Dictionary<string, Triplet[]> _tripletsInMemory;

        public AlgoInMemory(AsyncSaver saver, IWikidumpReader wikiReader, IMongoCollection<Triplet> triplets, string positionsPath) 
            : base(saver, wikiReader, triplets, positionsPath)
        {
            _tripletsInMemory = GetTripletsCache();
        }

        protected override void ProcessPair(PositionLine entity1, PositionLine entity2)
        {
            if (_tripletsInMemory.ContainsKey(entity1.EntityName + entity2.EntityName))
            {
                foreach (var t in _tripletsInMemory[entity1.EntityName + entity2.EntityName])
                {
                    ProcessTriplet(t, entity1, entity2, _wikiReader);
                    _positionsSet++;
                }
            }
            if (_tripletsInMemory.ContainsKey(entity2.EntityName + entity1.EntityName))
            {
                foreach (var t in _tripletsInMemory[entity2.EntityName + entity1.EntityName])
                {
                    ProcessTriplet(t, entity2, entity1, _wikiReader);
                    _positionsSet++;
                }
            }
        }

        private Dictionary<string, Triplet[]> GetTripletsCache()
        {
            var fields = Builders<Triplet>.Projection.Exclude(t => t.ArticlePositions[-1].Text);
            return _triplets
                .Find(r => true)
                .Project(fields)
                .As<Triplet>()
                .ToEnumerable()
                .GroupBy(t => t.ObjectWikiName + t.SubjectWikiName)
                .ToDictionary(t => t.Key, t => t.ToArray());
        }
    }
}
