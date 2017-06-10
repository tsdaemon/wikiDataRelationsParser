using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using MongoDB.Driver;
using PreprocessArticleTexts.Rules;

namespace PreprocessArticleTexts
{
    public class PreprocessService
    {
        private readonly List<IPreprocessRule> _rules;
        private readonly List<Property> _properties;
        private readonly IMongoCollection<Triplet> _triplets;
        private readonly IMongoCollection<TripletTrain> _tripletsTrain;

        public PreprocessService(List<IPreprocessRule> rules, List<Property> properties, IMongoCollection<Triplet> triplets, IMongoCollection<TripletTrain> tripletsTrain)
        {
            _rules = rules;
            _properties = properties;
            _triplets = triplets;
            _tripletsTrain = tripletsTrain;
        }

        public async Task<int> PreprocessTrain(Dictionary<string, int> stats)
        {
            var counter = 0;
            var triplets = _triplets.Find(t => t.ArticlePositions != null).ToCursor();
            await triplets.ForEachAsync(async t =>
            {
                foreach (var p in t.ArticlePositions)
                {
                    var tr = new TripletTrain
                    {
                        Object = t.SubjectWikiName,
                        ObjectAnchor = p.SubjectPosition.Anchor,

                        Subject = t.ObjectWikiName,
                        SubjectAnchor = p.ObjectPosition.Anchor,

                        Predicate = _properties.First(pr => pr.WikidataId == t.Property).ReadTitleUk,
                        PredicateId = t.Property,

                        Text = p.Text,
                        WikipediaLink = "https://uk.wikipedia.org/wiki/" + p.ArticleTitle,
                        WikipediaTitle = p.ArticleTitle
                    };

                    foreach (var rule in _rules)
                    {
                        rule.Preprocess(tr);
                        if (string.IsNullOrEmpty(tr.Text))
                        {
                            stats[rule.GetType().Name] += 1;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(tr.Text))
                    {
                        continue;
                    }

                    counter++;
                    await _tripletsTrain.InsertOneAsync(tr);
                }
            });
            return counter;
        }
    }
}
