using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetWikidataRelations.Model;
using MongoDB.Driver;

namespace UploadRelationPositions
{
    class Program
    {
        const string PositionsFilePath = "D:\\DRIVE\\ukr-ner\\linkz.csv\\linkz.csv";
        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");
        }
    }
}
