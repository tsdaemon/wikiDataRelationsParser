using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetWikidataRelations
{
    class Program
    {
        const string GET_RELATIONS_QUERY = 
        static void Main(string[] args)
        {
        }

        // person       Q4047087
        // organization Q43229
        // location     Q17334923
        // instance of  P31
        static void GetAllRelationTypesForTypes(string[] types, string outFile)
        {
            var pairs = new List<string[]>();
            for (var i = 0; i < types.Length; i++)
            {
                for (var j = 0; j < types.Length; j++)
                {
                    var pair = new[] {types[i], types[j]};
                    pairs.Add(pair);
                }
            }


        }
    }
}
