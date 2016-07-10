using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Core.Wikifile
{
    // reads only in forward direction
    public class WikidumpReader : IDisposable, IWikidumpReader
    {
        private StreamReader _reader;
        private string CurrentWikiTitle;
        private string CurrentText;
        private Regex xmlTag = new Regex("<(\\w+)>(.+)</(\\w+)>");
        private Regex xmlStartTag = new Regex("<(\\w+)[\\w\\s\\:\"\\=\\\\]+>(.+)");
        private Regex xmlEndTag = new Regex("(.+)</(\\w+)>");
        private string _file;

        public WikidumpReader(string file)
        {
            _file = file;
            _reader = openReader(file);
        }

        public string ExtractArticleText(string title)
        {
            var readed = 0;
            if (title != CurrentWikiTitle)
            {
                while (true)
                {
                    var line = _reader.ReadLine();
                    if (line == null)
                    {
                        readed++;
                        if (readed == 2)
                        {
                            // nothing to do there
                            return null;
                        }
                        _reader.Dispose();
                        _reader = openReader(_file);
                        continue;
                    }
                    if (xmlTag.IsMatch(line))
                    {
                        var match = xmlTag.Match(line);
                        if (match.Groups.Count > 2)
                        {
                            var tag = match.Groups[1].Value;
                            if (tag == "title")
                            {
                                var title2 = match.Groups[2].Value;
                                if (title2.Equals(title))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                var text = new StringBuilder();
                while (true)
                {
                    var line = _reader.ReadLine();
                    if (xmlStartTag.IsMatch(line))
                    {
                        var match = xmlStartTag.Match(line);
                        if (match.Groups.Count > 2)
                        {
                            var tag = match.Groups[1].Value;
                            if (tag == "text")
                            {
                                text.Append(match.Groups[2].Value);
                                break;
                            }
                        }
                    }
                }

                while (true)
                {
                    var line = _reader.ReadLine();
                    if (xmlEndTag.IsMatch(line))
                    {
                        var match = xmlEndTag.Match(line);
                        if (match.Groups.Count > 2)
                        {
                            var tag = match.Groups[2].Value;
                            if (tag == "text")
                            {
                                text.Append("\n" + match.Groups[1].Value);
                                break;
                            }
                        }
                    }
                    else
                    {
                        text.Append("\n" + line);
                    }
                }
                CurrentWikiTitle = title;
                CurrentText = HttpUtility.HtmlDecode(text.ToString());
            }
            return CurrentText;
        }

        public string ExtractArticleText(long pageid)
        {
            throw new NotImplementedException();
        }

        protected StreamReader openReader(string file)
        {
            return new StreamReader(File.OpenRead(file));
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
