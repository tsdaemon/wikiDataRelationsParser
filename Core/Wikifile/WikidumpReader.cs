using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Wikifile
{
    // reads only in forward direction
    public class WikidumpReader : IDisposable
    {
        private StreamReader _reader;
        private long CurrentWikiId;
        private string CurrentText;
        private Regex xmlTag = new Regex("<(\\w+)>(.+)");
        private Regex xmlEndTag = new Regex("(.+)</(\\w+)>");

        public WikidumpReader(string file)
        {
            _reader = new StreamReader(File.OpenRead(file));
        }

        public string ExtractArticleText(long id)
        {
            if (id != CurrentWikiId)
            {
                while (true)
                {
                    var line = _reader.ReadLine();
                    if (xmlTag.IsMatch(line))
                    {
                        var match = xmlTag.Match(line);
                        if (match.Groups.Count == 2)
                        {
                            var tag = match.Groups[0].Value;
                            if (tag == "id")
                            {
                                var id2 = long.Parse(match.Groups[1].Value);
                                if (id2 == id)
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
                    if (xmlTag.IsMatch(line))
                    {
                        var match = xmlTag.Match(line);
                        if (match.Groups.Count == 2)
                        {
                            var tag = match.Groups[0].Value;
                            if (tag == "text")
                            {
                                text.Append(match.Groups[1].Value);
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
                        if (match.Groups.Count == 2)
                        {
                            var tag = match.Groups[1].Value;
                            if (tag == "text")
                            {
                                text.Append(match.Groups[0].Value);
                                break;
                            }
                        }
                    }
                    else
                    {
                        text.Append(line);
                    }
                }
                CurrentWikiId = id;
                CurrentText = text.ToString();
            }
            return CurrentText;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
