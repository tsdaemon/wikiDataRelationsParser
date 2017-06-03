using System.Collections.Generic;
using System.Linq;
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

        public void PreprocessTrain()
        {
            var triplets = _triplets.Find(t => true).Limit(1000).ToCursor();
            triplets.ForEachAsync(t =>
            {
                foreach (var p in t.ArticlePositions)
                {
                    var tr = new TripletTrain
                    {
                        Object = t.SubjectWikiName,
                        Subject = t.ObjectWikiName,
                        Predicate = _properties.First(pr => pr.WikidataId == t.Property).ReadTitleUk,
                        Text = p.Text,
                        WikipediaLink = "https://uk.wikipedia.org/wiki/" + p.ArticleTitle,
                        WikipediaTitle = p.ArticleTitle
                    };

                    foreach (var rule in _rules)
                    {
                        rule.Preprocess(tr);
                        if (string.IsNullOrEmpty(tr.Text))
                        {
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(tr.Text))
                    {
                        continue;
                    }

                    _tripletsTrain.InsertOneAsync(tr);

                    // _tripletsTrain.ReplaceOneAsync(ttr => ttr.Id == tr.Id, tr, new UpdateOptions {IsUpsert=true});
                }
            });
        }
    }
}
