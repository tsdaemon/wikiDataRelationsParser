using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveTableRule : IPreprocessRule
    {
        private Regex r = new Regex(@"\s?\|\|\s?");

        public void Preprocess(TripletTrain result)
        {
            if (r.IsMatch(result.Text)) result.Text = null;
        }
    }
}
