using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GetWikidataRelations.Model
{
    public class Category
    {
        public ObjectId Id { get; set; }
        public string SectionId { get; set; }
        public string WikidataId { get; set; }
        public string Title { get; set; }
    }
}
