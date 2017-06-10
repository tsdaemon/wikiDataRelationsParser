using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveMarkupRule : IPreprocessRule
    {
        private Regex regex = new Regex(@"\&(\w+)\;|\{\{([^\}]+)\}\}|'{2,6}|[=\s]+External [lL]inks[\s=]+|[=\s]+See [aA]lso[\s=]+|[=\s]+References[\s=]+|[=\s]+Notes[\s=]+|\{\{([^\}]+)\}\}|\s\(\)|/={2,}/|<\/?\w+\s?\/?>|<\/?\w+\s?\/?|\/?\w+\s?\/?>" 
            + "|/{?class=\"[^\"]+\"/" 
            + "|/!?\\s?width=\"[^\"]+\"/" 
            + "|/!?\\s?height=\"[^\"]+\"/" 
            + "|/!?\\s?style=\"[^\"]+\"/"
            + "|/!?\\s?rowspan=\"[^\"]+\"/" 
            + @"|[Фф]айл\:.+\|");

        public void Preprocess(TripletTrain result)
        {
            result.Text = regex.Replace(result.Text, "");
        }
    }
}
