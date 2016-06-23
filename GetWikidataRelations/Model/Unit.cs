using System.Collections.Generic;
using MongoDB.Bson;

namespace GetWikidataRelations.Model
{
    public class UnitOfWork
    {
        public ObjectId Id { get; set; }
        public string SectionId { get; set; }

        public Category Category { get; set; }
        public Property Property { get; set; }

        public long Offset { get; set; }

        public bool Started { get; set; }
        public bool Done { get; set; }
    }
}
