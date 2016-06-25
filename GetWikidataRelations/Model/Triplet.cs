using System.Collections.Generic;
using MongoDB.Bson;

namespace GetWikidataRelations.Model
{
    public class Triplet
    {
        public ObjectId Id { get; set; }
        public string Property { get; set; }
        public string SectionId { get; set; }
        public string Category { get; set; }
        public Dictionary<string, string> WikiResult { get; set; }
        public string ObjectWikiName { get; set; }
        public string SubjectWikiName { get; set; }
    }
}
