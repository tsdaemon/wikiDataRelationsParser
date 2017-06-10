using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class SentenceLengthConstraints : IPreprocessRule
    {
        private readonly int _min;
        private readonly int _max;

        public SentenceLengthConstraints(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public void Preprocess(TripletTrain result)
        {
            var textLength = result.Text.Length;
            //var minimumLength = result.Object.Length + 8 + result.Subject.Length; // magic numbers woohoo
            if (textLength < _min || textLength > _max) result.Text = null;
        }
    }
}
