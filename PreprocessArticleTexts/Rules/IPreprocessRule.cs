using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public interface IPreprocessRule
    {
        void Preprocess(TripletTrain result);
    }
}
