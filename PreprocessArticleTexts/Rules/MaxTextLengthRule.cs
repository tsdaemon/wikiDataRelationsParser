using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class MaxTextLengthRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            if (result.Text.Length > 1000) result.Text = null;
        }
    }
}
