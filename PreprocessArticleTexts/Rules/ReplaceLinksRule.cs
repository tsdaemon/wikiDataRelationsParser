using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class ReplaceLinksRule : IPreprocessRule
    {
        private Regex link = new Regex(@"\[\[(?:[^|\]]*\|)?([^\]]+)\]\]|\[(?:[^|\]]*\|)?([^\]]+)\]\]|\[\[(?:[^|\]]*\|)?([^\]]+)\]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

        public void Preprocess(TripletTrain result)
        {
            result.Text = link.Replace(result.Text, "$1$2$3");
        }
    }
}
