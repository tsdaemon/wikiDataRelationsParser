using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace WikidumpToFiles
{
    class Program
    {
        const string WikidumpPath = "D:\\DB\\ukwiki\\ukwiki-20160601-pages-articles.xml";
        const string OutputPath = "C:\\DB\\uk-wiki\\";
        static void Main(string[] args)
        {
            using (var reader = XmlReader.Create(new StreamReader(File.OpenRead(WikidumpPath), Encoding.UTF8), new XmlReaderSettings {}))
            {
                var done = 0;
                var doneDictionary = new Dictionary<string, bool>();
                while (!reader.EOF)
                {
                    reader.ReadToFollowing("page");
                    if (reader.EOF) break;

                    reader.ReadToDescendant("id");
                    if (reader.EOF) break;
                    var fileName = OutputPath +  PathHelper.EncodeFilePathPart(reader.ReadElementContentAsString()) + ".txt";

                    reader.ReadToFollowing("text");
                    if (reader.EOF) break;
                    var content = reader.ReadElementContentAsString();

                    WaitCallback worker = async state => 
                    {
                        await WriteFile(content, fileName, doneDictionary);
                    };
                    if (!doneDictionary.ContainsKey(fileName))
                    {
                        doneDictionary.Add(fileName, false);
                    }

                    ThreadPool.QueueUserWorkItem(worker);
                    done++;

                    if (done % 10000 == 0) writeDone(doneDictionary);  
                }
                while (!doneDictionary.Values.ToArray().All(d => d))
                {
                    writeDone(doneDictionary);
                    Thread.Sleep(2000);
                }
            }
        }

        private static void writeDone(Dictionary<string, bool> doneDictionary)
        {
            var copy = doneDictionary.Values.ToArray();
            Console.WriteLine("Done {0}/{1}", copy.Count(d => d), copy.Length);
        }

        static string[] words = new[] { "#ПЕРЕНАПРАВЛЕННЯ", "#REDIRECT" };
        static async Task<bool> WriteFile(string content, string fileName, Dictionary<string, bool> doneDictionary)
        {
            
            if (words.All(w => content.ToUpper().IndexOf(w, StringComparison.Ordinal) != 0) && !File.Exists(fileName))
            {
                try
                {
                    using (var wr = new StreamWriter(File.OpenWrite(fileName), Encoding.UTF8))
                    {
                        await wr.WriteAsync(content);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FileName {0} Exception {1}", fileName, ex);
                }
            }
            doneDictionary[fileName] = true;
            return true;
        }
    }

    
}
