using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GetWikidataRelations.Model;
using MongoDB.Driver;

namespace ParseTripletsIds
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");
            var count = triplets.Count(t => t.SubjectWikiName == null || t.ObjectWikiName == null);
            var done = 0;
            foreach(var t in triplets.Find(t => t.SubjectWikiName == null || t.ObjectWikiName == null).ToEnumerable())
            {
                var wikiresult = t.WikiResult;
                t.ObjectWikiName = Uri.UnescapeDataString(wikiresult["objectWiki"].Split('/').Last());
                t.SubjectWikiName = Uri.UnescapeDataString(wikiresult["subjectWiki"].Split('/').Last());
                triplets.ReplaceOne(t2 => t.Id == t2.Id, t);
                done++;
                if (done%500 == 0)
                {
                    Console.WriteLine("{0}/{1} done", done, count);
                }
            }
        }
    }
}
