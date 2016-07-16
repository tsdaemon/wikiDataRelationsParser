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

        protected override int ProcessPair(PositionLine entity1, PositionLine entity2)
        {
            var p = 0;
            foreach (var t in _triplets
                .Find(t => t.ObjectWikiName == entity1.EntityName && t.SubjectWikiName == entity2.EntityName)
                .ToEnumerable())
            {
                ProcessTriplet(t.Id, entity1, entity2, _wikiReader);
                p++;

            }

            foreach (var t in _triplets
                .Find(t => t.ObjectWikiName == entity2.EntityName && t.SubjectWikiName == entity1.EntityName)
                .ToEnumerable())
            {
                ProcessTriplet(t.Id, entity2, entity1, _wikiReader);
                p++;

            }
            return p;
        }
    }
}
