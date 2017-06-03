﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Core;
using Core.Model;
using Core.Service;
using Core.Wikifile;
using MongoDB.Driver;
using UploadRelationPositionsIntersect.Algo;

namespace UploadRelationPositionsIntersect
{
    class Program
    {
        private static int _startOffset;
        private static int _count;
        private static int _offset;
        private static Stopwatch _timer;

        const string PositionsFilePath = "links.csv";

        const string OffsetFilePath = "offset.txt";
        const string CountFilePath = "count.txt";
        private static int _positionsSet;
        private static AsyncSaver _asyncSaver;

        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");

            var config = new PathConfiguration();
            
            _asyncSaver = new AsyncSaver(triplets);

            _count = GetLinesCount(config.GetPath(PositionsFilePath), config.GetPath(CountFilePath));
            _offset = GetOffset(config.GetPath(OffsetFilePath));
            _positionsSet = 0;
            _startOffset = _offset;
            _timer = new Stopwatch();

            using (var wikiFile = new WikidumpReader(config.WikipediaPath))
            {
                var algo = new AlgoInMemory(_asyncSaver, wikiFile, triplets, PositionsFilePath);

                _timer.Start();
                algo.OnProcessed += algo_OnProcessed;
                algo.Process(_offset);
            }
            
        }

        static void algo_OnProcessed(AlgoProcessedEvent ev)
        {
            _offset += ev.LinesDone;
            _positionsSet += ev.PositionsSet;
            Report(_offset, _offset-_startOffset, _count, _positionsSet);

            try
            {
                using (var o = new StreamWriter(File.Open(OffsetFilePath, FileMode.Create)))
                {
                    o.Write(_offset);
                }
            }
            catch
            {

            }
        }

        private static void Report(int offset, int processed, int count, int positionsSet)
        {
            var estimation = TimeSpan.FromMilliseconds(_timer.ElapsedMilliseconds * ((count - offset) / processed));
            Console.WriteLine("{0} done, estimation {1}, positions set {2}, in saving queue {3}", offset, estimation.ToString(@"dd\:hh"), positionsSet, _asyncSaver.InQueue);
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
    }
}
