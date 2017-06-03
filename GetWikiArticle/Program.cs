using System.IO;
using Core.Wikifile;

namespace GetWikiArticle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new WikidumpReader("H:\\ukr-ner\\ukwiki-20160601-pages-articles.xml"))
            {
                var article = reader.ExtractArticleText("Нумідія");
                using (var streamWriter = new StreamWriter(File.OpenWrite("../../../Core.Tests/Нумідія.txt")))
                {
                    streamWriter.Write(article);
                }
            }
        }
    }
}
