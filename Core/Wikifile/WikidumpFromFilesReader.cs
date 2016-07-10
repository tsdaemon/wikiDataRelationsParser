using System;
using System.IO;
using System.Text;

namespace Core.Wikifile
{
    // reads only in forward direction
    public class WikidumpFromFilesReader : IDisposable, IWikidumpReader
    {
        private string _dir;

        public WikidumpFromFilesReader(string dir)
        {
            _dir = dir;
        }

        public void Dispose()
        {
        }

        public string ExtractArticleText(string title)
        {
            var fileName = _dir + PathHelper.EncodeFilePathPart(title) + ".txt";
            using (var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public string ExtractArticleText(long pageid)
        {
            var fileName = _dir + pageid + ".txt";
            if (File.Exists(fileName))
            {
                using (var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;
        }
    }
}
