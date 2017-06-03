using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Wikifile;

namespace GetWikipediaTextsForTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new FileConfiguration();
            using (var reader = new WikidumpReader(config.WikipediaPath))
            {
                
            };
        }
    }
}
