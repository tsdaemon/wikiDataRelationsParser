using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Core.Model;
using MongoDB.Driver;
using PreprocessArticleTexts.Rules;

namespace PreprocessArticleTexts
{
    public class Program
    {
        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");

            var triplets = db.GetCollection<Triplet>("triplet");
            var properties = db.GetCollection<Property>("property").Find(a => true).ToList();
            var tripletsTrain = db.GetCollection<TripletTrain>("triplet_train");
            tripletsTrain.DeleteMany(s => true);

            var rules = new List<IPreprocessRule>
            {
                new GenerateIdRule(),

                new ForbiddenSymbolsRule(),
                new MoreThenOneLineBreakRule(),

                new RemoveTemplateRule(),
                new RemoveListRule(),

                new ReplaceLinksRule(),
                new ReplaceSpaceRule(),

                new RemoveMarkupRule(),
                new RemoveMarkupRule(),

                new TrimRule()
            };

            var service = new PreprocessService(rules, properties, triplets, tripletsTrain);
            service.PreprocessTrain();
            Console.WriteLine("Almost done. Wait till async finished");
            Console.ReadKey();
        }
    }
}
