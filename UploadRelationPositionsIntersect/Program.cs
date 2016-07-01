using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Model;
using Core.Wikifile;
using LINQtoCSV;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect
{
    class Program
    {
        const string PositionsFilePath = "D:\\DRIVE\\ukr-ner\\linkz.csv\\linkz.csv";
        const string WikidumpPath = "D:\\DB\\ukwiki\\ukwiki-20160601-pages-articles.xml";

        const string OffsetFilePath = "offset.txt";
        const string CountFilePath = "count.txt";

        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");
            var count = GetLinesCount(PositionsFilePath, CountFilePath);
            var offset = GetOffset(OffsetFilePath);

            var lines = PrepareCsvReader(PositionsFilePath);

            var positionsSet = 0;
            using (var wikiFile = new WikidumpReader(WikidumpPath))
            {
                foreach (var g in lines.Skip(offset).GroupBySequentually(l => l.WikiTitle))
                {
                    var values = g.ToArray();
                    // go through all entity combinations
                    for (var i = 0; i < values.Length; i++)
                    {
                        for (var j = i + 1; j < values.Length; j++)
                        {
                            var entity1 = values[i];
                            var entity2 = values[j];
                            if (entity1.EntityName != entity2.EntityName)
                            {
                                foreach (var t in triplets.Find(t => t.ObjectWikiName == entity1.EntityName
                                                                     && t.SubjectWikiName == entity2.EntityName)
                                    .ToEnumerable())
                                {
                                    ProcessTriplet(t, entity1, entity2, triplets, wikiFile);
                                }
                                offset = IncrementOffset(offset, count, positionsSet);
                            }
                        }
                    }
                }
            }
        }

        private static void ProcessTriplet(Triplet t, 
            PositionLine object_, 
            PositionLine subject,
            IMongoCollection<Triplet> triplets, WikidumpReader reader)
        {
            if (t.ArticlePositions == null) t.ArticlePositions = new List<AnotherArticlePosition>();
            var position = new AnotherArticlePosition
            {
                ArticleTitle = object_.WikiTitle,
                ArticleId = object_.PageId,
                ObjectPosition = object_.ToPosition(),
                SubjectPosition = subject.ToPosition()
            };
            if (t.ArticlePositions.Contains(position)) return;

            var text = reader.ExtractArticleText(object_.PageId);

            t.ArticlePositions.Add(position);
            triplets.ReplaceOne(t2 => t2.Id == t.Id, t);
        }

        private static int IncrementOffset(int offset, int count, int positionsSet)
        {
            offset++;
            if (offset%1000 == 0)
            {
                Console.WriteLine("{0}/{1} done, positions set {2}", offset, count, positionsSet);
                using (var o = new StreamWriter(File.Open(OffsetFilePath, FileMode.Create)))
                {
                    o.Write(offset);
                }
            }
            return offset;
        }

        // estimate lines count in file or read from saved file
        private static int GetLinesCount(string file, string fileCount)
        {
            if (File.Exists("count.txt"))
            {
                using (var f = new StreamReader(File.OpenRead(fileCount)))
                {
                    return int.Parse(f.ReadToEnd());
                }
            }
            else
            {
                using (var f = new StreamReader(File.OpenRead(file)))
                {
                    var count = f.GetLines().Skip(1).Count();
                    using (var f2 = new StreamWriter(File.Open(fileCount,FileMode.Create)))
                    {
                        f2.WriteLine(count);
                    }
                    return count;
                }
            }
        }

        private static int GetOffset(string offsetFile)
        {
            if (File.Exists(offsetFile))
            {
                using (var o = new StreamReader(File.OpenRead(offsetFile)))
                {
                    return int.Parse(o.ReadToEnd().Trim());
                }
            }
            return 0;
        }

        private static IEnumerable<PositionLine> PrepareCsvReader(string file)
        {
            var inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            var cc = new CsvContext();
            return cc.Read<PositionLine>(file, inputFileDescription);
        }
    }
}
