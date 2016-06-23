using System.Collections.Generic;
using System.IO;
using System.Linq;
using GetWikidataRelations.Model;
using MongoDB.Driver;

namespace ReloadConfigFromFile
{
    class Program
    {
        static List<string> ids = new List<string>
        {
           "person", "organization", "location"
        };

        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var properties = db.GetCollection<Property>("property");
            var categories = db.GetCollection<Category>("category");
            var units = db.GetCollection<UnitOfWork>("unit");

            units.DeleteMany(f => true);

            foreach (var id in ids)
            {
                var categoriesFile = "../../../categories_" + id + ".txt";
                var propertiesFile = "../../../properties_" + id + ".txt";
        
                using (var stream = new StreamReader(File.OpenRead(categoriesFile)))
                {
                    categories.InsertMany(stream.ReadToEnd()
                        .Split('\n')
                        .Where(s => !(string.IsNullOrEmpty(s) || s[0] == '#'))
                        .Select(s => new Category
                                {
                                    SectionId = id,
                                    Title = s.Trim().Split('\t')[1],
                                    WikidataId = s.Trim().Split('\t')[0]
                                }));
                }
                using (var stream = new StreamReader(File.OpenRead(propertiesFile)))
                {
                    properties.InsertMany(stream.ReadToEnd()
                        .Split('\n')
                        .Where(s => !(string.IsNullOrEmpty(s) || s[0] == '#'))
                        .Select(s => new Property
                        {
                            SectionId = id,
                            Title = s.Trim().Split('\t')[1],
                            WikidataId = s.Trim().Split('\t')[0],
                            Qualifier = s.Trim().Split('\t')[2]
                        }));
                }
            }

        }
    }
}
