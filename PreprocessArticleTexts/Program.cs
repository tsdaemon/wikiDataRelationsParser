using System;
using System.Collections.Generic;
using System.Linq;
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
                new RemoveUselessPredicates(),

                new GenerateIdRule(),

                new RemoveDuplicatesRule(),

                new ForbiddenSymbolsRule(),
                new MoreThenOneLineBreakRule(),

                new RemoveTemplateRule(),
                new RemoveListRule(),
                new RemoveTableRule(),
                new RemoveImageRule(),

                new ReplaceLinksRule(),
                new ReplaceSpaceRule(),

                new RemoveMarkupRule(),
                new RemoveMarkupRule(),

                new ReplaceWeirdCharactersRule(),

                new SentenceLengthConstraints(30,2000),

                new CheckObjectAndSubjectStillHereRule(),

                new TrimRule(),
            };
            var removeStats = rules.Select(r => r.GetType().Name).Distinct().ToDictionary(s => s, s => 0);

            var service = new PreprocessService(rules, properties, triplets, tripletsTrain);
            var added = service.PreprocessTrain(removeStats);
            added.Wait();
            Console.WriteLine($"Added {added.Result} records");
            Console.ReadKey();
        }
    }
}
