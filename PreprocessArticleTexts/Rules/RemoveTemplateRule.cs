using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveTemplateRule : IPreprocessRule
    {
        private Regex r = new Regex(@"\|[\w\s\d'а-яєіїА-ЯЄIЇ]+\s?=\s*\[\[[\w\s\d'а-яєіїА-ЯЄIЇ\|\)\(]+\]\]");

        public void Preprocess(TripletTrain result)
        {
            result.Text = r.Replace(result.Text, "");
        }
    }
}
