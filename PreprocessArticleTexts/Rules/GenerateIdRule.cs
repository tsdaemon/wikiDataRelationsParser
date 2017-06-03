using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PreprocessArticleTexts.Rules
{
    public class GenerateIdRule : IPreprocessRule
    {
        public void Preprocess(TripletTrain result)
        {
            result.Id = Guid.NewGuid().ToString();
        }
    }
}
