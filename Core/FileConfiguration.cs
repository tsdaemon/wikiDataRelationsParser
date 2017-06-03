using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class PathConfiguration
    {
        public const string WORKING_DIR = "H:\\ukr-ner\\";

        public string GetPath(string path)
        {
            return WORKING_DIR + path;
        }

        public string WikipediaPath => WORKING_DIR + "ukwiki-20160601-pages-articles.xml";

        public string PositionsPath => WORKING_DIR + "links.csv";
    }
}
