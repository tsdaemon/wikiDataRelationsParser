using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class MoreThenOneLineBreakRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            var lineBreaksCount = result.Text.Count(c => c == '\n');
            if (lineBreaksCount > 1) result.Text = null;
        }
    }
}
