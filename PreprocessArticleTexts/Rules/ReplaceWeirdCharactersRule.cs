using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class ReplaceWeirdCharactersRule : IPreprocessRule
    {
        private Regex regex = new Regex(@"\(\,\s?\)\s?|\*\s?|\s\,\s|\(\)");
        private Regex regex2 = new Regex(@"\(\;\s?|\(\,\s?");
        private Regex regex3 = new Regex(@"\;\s?\)|\,\s?\)");

        public void Preprocess(TripletTrain result)
        {
            result.Text = regex.Replace(result.Text, "");
            result.Text = regex2.Replace(result.Text, "(");
            result.Text = regex3.Replace(result.Text, ")");
        }
    }
}
