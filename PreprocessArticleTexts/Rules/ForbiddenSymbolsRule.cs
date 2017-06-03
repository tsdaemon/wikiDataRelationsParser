using System.Text.RegularExpressions;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class ForbiddenSymbolsRule : IPreprocessRule
    {
        private Regex symbols = new Regex(@"\n|=|→");

        public void Preprocess(TripletTrain result)
        {
            if (symbols.IsMatch(result.Text)) result.Text = null;
        }
    }
}
