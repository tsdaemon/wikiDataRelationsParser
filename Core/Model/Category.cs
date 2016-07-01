using MongoDB.Bson;

namespace Core.Model
{
    public class Category
    {
        public ObjectId Id { get; set; }
        public string SectionId { get; set; }
        public string WikidataId { get; set; }
        public string Title { get; set; }
    }
}
