using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class ReplaceSpaceRule : IPreprocessRule
    {
        private Regex regex = new Regex(@"\&nbsp\;");

        public void Preprocess(TripletTrain result)
        {
            result.Text = regex.Replace(result.Text, " ");
        }
    }
}
