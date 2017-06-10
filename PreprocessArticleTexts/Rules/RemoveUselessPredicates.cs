using Core.Model;
using System.Linq;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveUselessPredicates : IPreprocessRule
    {
        private string[] ids = {"P190", "P47"};

        public void Preprocess(TripletTrain result)
        {
            if (ids.Contains(result.PredicateId)) result.Text = null;
        }
    }
}
