using System.IO;
using Core.Model;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DumpRelationTexts
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");

            var triplets = db.GetCollection<TripletTrain>("triplet_train");

            var train = triplets.Find(t => t.Label == RelationLabel.Unknown).Limit(1000).ToList();

            // serialize JSON to a string and then write string to a file
            File.WriteAllText(@"dump.json", JsonConvert.SerializeObject(train));
        }
    }
}
