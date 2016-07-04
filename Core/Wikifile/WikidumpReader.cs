﻿using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.Wikifile
{
    // reads only in forward direction
    public class WikidumpReader : IDisposable
    {
        private StreamReader _reader;
        private string CurrentWikiTitle;
        private string CurrentText;
        private Regex xmlTag = new Regex("<(\\w+)>(.+)</(\\w+)>");
        private Regex xmlEndTag = new Regex("(.+)</(\\w+)>");

        public WikidumpReader(string file)
        {
            _reader = new StreamReader(File.OpenRead(file));
        }

        public string ExtractArticleText(string title)
        {
            if (title != CurrentWikiTitle)
            {
                var builder = new StringBuilder();
                while (true)
                {
                    var line = _reader.ReadLine();
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
                                    builder.Append("  <page>\r\n");
                                    builder.Append(line + "\r\n");
                                    break;
                                }
                            }
                        }
                    }
                }

                while (true)
                {
                    var line = _reader.ReadLine();
                    builder.Append(line + "\r\n");
                    if (xmlEndTag.IsMatch(line))
                    {
                        var match = xmlEndTag.Match(line);
                        if (match.Groups.Count > 2)
                        {
                            var tag = match.Groups[2].Value;
                            if (tag == "page")
                            {
                                break;
                            }
                        }
                    }
                }

                CurrentWikiTitle = title;
                CurrentText = builder.ToString();
            }
            return CurrentText;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}