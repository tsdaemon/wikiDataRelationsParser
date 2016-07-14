using Core.Model;
using Core.Service;
using Core.Wikifile;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect.Algo
{
    public class AlgoInDb : AlgoBase
    {
        public AlgoInDb(AsyncSaver saver, IWikidumpReader wikiReader, IMongoCollection<Triplet> triplets, string positionsPath) 
            : base(saver, wikiReader, triplets, positionsPath)
        {
        }

        protected override void ProcessPair(PositionLine entity1, PositionLine entity2)
        {
            foreach (var t in _triplets
                .Find(t => t.ObjectWikiName == entity1.EntityName && t.SubjectWikiName == entity2.EntityName)
                .ToEnumerable())
            {
                ProcessTriplet(t, entity1, entity2, _wikiReader);
                _positionsSet++;

            }

            foreach (var t in _triplets
                .Find(t => t.ObjectWikiName == entity2.EntityName && t.SubjectWikiName == entity1.EntityName)
                .ToEnumerable())
            {
                ProcessTriplet(t, entity2, entity1, _wikiReader);
                _positionsSet++;

            }
        }
    }
}
