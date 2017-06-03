using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveListRule : IPreprocessRule
    {
        private Regex r = new Regex(@"\[\[[\w\s\d'а-яєіїА-ЯЄIЇ\|\)\(]+\]\]|\s\*\s|\;\s|\,\s|\s?\|\|\s?|\'\'|—", 
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public void Preprocess(TripletTrain result)
        {
            var t = r.Replace(result.Text, "");
            if (string.IsNullOrEmpty(t)) result.Text = null; 
        }
    }
}
