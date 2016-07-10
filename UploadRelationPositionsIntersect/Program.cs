using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Core;
using Core.Model;
using Core.Service;
using Core.Wikifile;
using LINQtoCSV;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect
{
    class Program
    {
        private static Stopwatch _time;
        private static int _startOffset;
        private static IMongoCollection<Triplet> _triplets;
        private static Dictionary<string, Triplet[]> _tripletsInMemory;
        private static AsyncSaver _asyncSaver;

        const string PositionsFilePath = "D:\\DRIVE\\ukr-ner\\linkz.csv\\linkz.csv";
        const string WikidumpPath = "D:\\DB\\ukwiki\\ukwiki-20160601-pages-articles.xml";

        const string OffsetFilePath = "offset.txt";
        const string CountFilePath = "count.txt";

        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            _triplets = db.GetCollection<Triplet>("triplet");
            _tripletsInMemory = _triplets.Find(r => true)
                                         .ToEnumerable()
                                         .GroupBy(t => t.ObjectWikiName + t.SubjectWikiName)
                                         .ToDictionary(t => t.Key, t => t.ToArray());
            _asyncSaver = new AsyncSaver(_triplets);

            var count = GetLinesCount(PositionsFilePath, CountFilePath);
            var offset = GetOffset(OffsetFilePath);
            _startOffset = offset;

            var lines = PrepareCsvReader(PositionsFilePath);

            var positionsSet = 0;
            _time = new Stopwatch();
            _time.Start();
            using (var wikiFile = new WikidumpReader(WikidumpPath))
            {
                foreach (var g in lines.Skip(offset).GroupBySequentually(l => l.PageId))
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
                                if (_tripletsInMemory.ContainsKey(entity1.EntityName + entity2.EntityName))
                                {
                                    foreach (var t in _tripletsInMemory[entity1.EntityName + entity2.EntityName])
                                    {
                                        ProcessTriplet(t, entity1, entity2, wikiFile);
                                        positionsSet++;
                                    }
                                }
                                if (_tripletsInMemory.ContainsKey(entity2.EntityName + entity1.EntityName))
                                {
                                    foreach (var t in _tripletsInMemory[entity2.EntityName + entity1.EntityName])
                                    {
                                        ProcessTriplet(t, entity2, entity1, wikiFile);
                                        positionsSet++;
                                    }
                                }
                            }
                        }
                    }
                    offset = IncrementOffset(offset, values.Length, count, positionsSet);
                }
            }
        }

        private static void ProcessTriplet(Triplet t, 
            PositionLine object_, 
            PositionLine subject,
            WikidumpReader reader)
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

            var text = reader.ExtractArticleText(object_.WikiTitle);
            if (text == null) return;
            var startPosition = object_.Start < subject.Start ? object_.Start : subject.Start;
            var endPosition = object_.End > subject.End ? object_.End : subject.End;

            int newStart;
            int newEnd;
            position.Text = TextHelper.ExtractTextWithSentenceWindow(text, startPosition, endPosition, out newStart,
                out newEnd);
            position.Start = newStart;
            position.End = newEnd;
            position.Distance = newEnd - newStart;

            t.ArticlePositions.Add(position);
            
            _asyncSaver.Save(t);
        }

        private static int IncrementOffset(int offset, int processed, int count, int positionsSet)
        {
            offset += processed;
            var time = _time.Elapsed;
            var estimation = TimeSpan.FromMilliseconds(_time.ElapsedMilliseconds*(count/(offset - _startOffset)));
            Console.WriteLine("{0}/{1} done, elapsed {2}, estimation {3}, positions set {4}", offset, count, time.ToString(@"hh\:mm\:ss"), estimation.ToString(@"dd\:hh"), positionsSet);
            using (var o = new StreamWriter(File.Open(OffsetFilePath, FileMode.Create)))
            {
                o.Write(offset);
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
