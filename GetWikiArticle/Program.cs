using System.IO;
using Core.Wikifile;

namespace GetWikiArticle
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var reader = new WikidumpReader("D:\\DB\\ukwiki\\ukwiki-20160601-pages-articles.xml"))
            {
                var article = reader.ExtractArticleText("Менделєєв Дмитро Іванович");
                using (var streamWriter = new StreamWriter(File.OpenWrite("../../715_orig_sharp.txt")))
                {
                    streamWriter.Write(article);
                }
            }
        }
    }
}
