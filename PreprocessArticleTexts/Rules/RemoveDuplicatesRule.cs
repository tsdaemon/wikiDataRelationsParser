using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class RemoveDuplicatesRule : IPreprocessRule
    {
        private HashSet<int> tripletHashes = new HashSet<int>();

        public void Preprocess(TripletTrain result)
        {
            var hash =
            (result.Object + result.ObjectAnchor + result.Subject + result.SubjectAnchor + result.PredicateId +
             result.Text).GetHashCode();
            if (tripletHashes.Contains(hash))
            {
                result.Text = null;
            }
            else
            {
                tripletHashes.Add(hash);
            }
        }
    }
}
