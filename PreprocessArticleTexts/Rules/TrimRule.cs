using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class TrimRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            result.Text = result.Text.Trim();
        }
    }
}
