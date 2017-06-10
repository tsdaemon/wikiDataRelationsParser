using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class LevenshteinDistanceRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            var objDistance = LevenshteinDistanceNormalized(result.Object, result.ObjectAnchor);
            var subjDistance = LevenshteinDistanceNormalized(result.Subject, result.SubjectAnchor);
        }

        private double LevenshteinDistanceNormalized(string a, string b)
        {
            a = a.ToLower();
            b = b.ToLower();
            var m = Math.Min(a.Length, b.Length);
            var distance = 0.0;
            for (var i = 0; i < m; i++)
            {
                if (a[i] != b[i]) distance++;
            }
            return distance / m;
        }
    }
}
