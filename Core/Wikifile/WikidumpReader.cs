using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace Core.Wikifile
{
    // reads only in forward direction
    public class WikidumpReader : IDisposable, IWikidumpReader
    {
        private XmlReader _reader;
        private string CurrentWikiTitle;
        private int CurrentWikiId;
        private string CurrentText;
        private string _file;
        private int _parsed;

        public WikidumpReader(string file)
        {
            _file = file;
            _reader = openReader(file);
        }

        public string ExtractArticleText(string title)
        {
            _parsed = 0;
            if (title != CurrentWikiTitle)
            {
                while (title != CurrentWikiTitle && CheckEnd())
                {
                    _reader.ReadToFollowing("page");

                    _reader.ReadToDescendant("title");
                    CurrentWikiTitle = _reader.ReadElementContentAsString();
                }
                _reader.ReadToFollowing("text");
                CurrentText = _reader.ReadElementContentAsString();
            }
            return CurrentText;
        }

        public string ExtractArticleText(long pageid)
        {
            _parsed = 0;
            if (pageid != CurrentWikiId)
            {
                while (pageid != CurrentWikiId && CheckEnd())
                {
                    _reader.ReadToFollowing("page");

                    _reader.ReadToDescendant("id");
                    CurrentWikiId = _reader.ReadElementContentAsInt();
                }
                _reader.ReadToFollowing("text");
                CurrentText = _reader.ReadElementContentAsString();
            }
            return CurrentText;
        }

        private bool CheckEnd()
        {
            if (_reader.EOF)
            {
                _reader = openReader(_file);
                if (_parsed > 0)
                    return false;
                else
                {
                    _parsed++;
                    return true;
                }
            }
            return true;
        }

        private XmlReader openReader(string file)
        {
            return XmlReader.Create(new StreamReader(File.OpenRead(file), Encoding.UTF8), new XmlReaderSettings {});
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
