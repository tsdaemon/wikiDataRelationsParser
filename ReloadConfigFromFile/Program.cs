using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Model;
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
            var units = db.GetCollection<UnitPart>("unit");

            units.DeleteMany(f => true);
            properties.DeleteMany(f => true);
            categories.DeleteMany(f => true);

            foreach (var id in ids)
            {
                var categoriesFile = "../../../categories_" + id + ".txt";
                var propertiesFile = "../../../properties_" + id + ".txt";
        
                using (var stream = new StreamReader(File.OpenRead(categoriesFile)))
                {
                    categories.InsertMany(stream.ReadToEnd()
                        .Split('\n')
                        .Where(s => !(string.IsNullOrEmpty(s) || s[0] == '#'))
                        .Select(s =>
                        {
                            var vv = s.Trim().Split('\t');
                            var title = vv.Length > 1 ? vv[1] : null;
                            return new Category
                            {
                                SectionId = id,
                                Title = title,
                                WikidataId = vv[0]
                            };
                        }));
                }
                using (var stream = new StreamReader(File.OpenRead(propertiesFile)))
                {
                    properties.InsertMany(stream.ReadToEnd()
                        .Split('\n')
                        .Where(s => !(string.IsNullOrEmpty(s) || s[0] == '#'))
                        .Select(s =>
                        {
                            var vv = s.Trim().Split('\t');
                            var qualifier = vv.Length > 2 ? vv[2] : null;
                            var title = vv.Length > 1 ? vv[1] : null;
                            return new Property
                            {
                                SectionId = id,
                                Title = title,
                                WikidataId = vv[0],
                                Qualifier = qualifier
                            };
                        }))
                ;
                }
            }

        }
    }
}
