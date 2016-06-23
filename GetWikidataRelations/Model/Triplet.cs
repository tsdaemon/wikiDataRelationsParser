using System.Collections.Generic;
using MongoDB.Bson;

namespace GetWikidataRelations.Model
{
    public class Triplet
    {
        public ObjectId Id { get; set; }
        public Property Property { get; set; }
        public string SectionId { get; set; }
        public Category Category { get; set; }
        public Dictionary<string, string> WikiResult { get; set; }
    }
}
