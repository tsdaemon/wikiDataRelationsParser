using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class CheckObjectAndSubjectStillHereRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            var subjectHere = result.Text.Contains(result.SubjectAnchor);
            var objectHere = result.Text.Contains(result.ObjectAnchor);
            if (!subjectHere || !objectHere) result.Text = null;
        }
    }
}
