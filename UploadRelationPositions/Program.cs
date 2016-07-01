using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using Core.Model;
using Core.Wikifile;
using LINQtoCSV;
using MongoDB.Driver;

namespace UploadRelationPositions
{
    class Program
    {
        const string PositionsFilePath = "D:\\DRIVE\\ukr-ner\\linkz.csv\\linkz.csv";
        const string OffsetFilePath = "offset.txt";

        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");
            var count = 0;
            // estimate lines count in file
            using (var f = new StreamReader(File.OpenRead(PositionsFilePath)))
            {
                count = f.GetLines().Skip(1).Count();
            }
            // read offset
            var offset = 0;
            if (File.Exists(OffsetFilePath))
            {
                using (var o = new StreamReader(File.OpenRead(OffsetFilePath)))
                {
                    offset = int.Parse(o.ReadToEnd().Trim());
                }
            }
            // start reconcilation
            CsvFileDescription inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext cc = new CsvContext();
            var lines =
                cc.Read<PositionLine>(PositionsFilePath, inputFileDescription);

            var positionsSet = 0;
            foreach (var line in lines.Skip(offset))
            {
                var objectWiki = line.WikiTitle;
                var subjectWiki = line.EntityName;

                foreach (var t in triplets.Find(t => t.ObjectWikiName == objectWiki && t.SubjectWikiName == subjectWiki).ToEnumerable())
                {
                    if (t.Positions == null) t.Positions = new List<Position>();
                    t.Positions.Add(new Position
                    {
                        Anchor = string.IsNullOrEmpty(line.Anchor) ? line.EntityName : line.Anchor,
                        End = line.End,
                        Start = line.Start
                    });
                    triplets.ReplaceOne(t2 => t.Id == t2.Id, t);
                    positionsSet++;
                }
                offset++;
                if (offset % 1000 == 0)
                {
                    Console.WriteLine("{0}/{1} done, positions set {2}", offset, count, positionsSet);
                    using (var o = new StreamWriter(File.Open(OffsetFilePath,FileMode.Create)))
                    {
                        o.Write(offset);
                    }
                }
            }
        }
    }
}
