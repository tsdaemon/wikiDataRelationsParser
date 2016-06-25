using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GetWikidataRelations.Model;
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
                count = GetLines(f).Skip(1).Count();
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
            using (var f = new StreamReader(File.OpenRead(PositionsFilePath),Encoding.UTF8))
            {
                var positionsSet = 0;
                var linesBroken = 0;
                var en = GetLines(f).Skip(offset + 1);
                foreach (var line in en)
                {
                    var values = Regex.Split(line, @"""\s*,\s*""");
                    if (values.Length != 6)
                    {
                        Console.WriteLine("Line broken: {0}", line);
                        linesBroken++;
                        continue;
                    }

                    var objectWiki = values[0];
                    var subjectWiki = values[2];

                    foreach (var t in triplets.Find(t => t.ObjectWikiName == objectWiki && t.SubjectWikiName == subjectWiki).ToEnumerable())
                    {
                        t.Positions.Add(new Position
                        {
                            Anchor = string.IsNullOrEmpty(values[3]) ? values[2] : values[3],
                            End = int.Parse(values[5]),
                            Start = int.Parse(values[4])
                        });
                        triplets.ReplaceOne(t2 => t.Id == t2.Id, t);
                        positionsSet++;
                    }
                    offset++;
                    if (offset % 1000 == 0)
                    {
                        Console.WriteLine("{0}/{1} done, positions set {2}, broken lines {3}", offset, count, positionsSet, linesBroken);
                        using (var o = new StreamWriter(File.Open(OffsetFilePath,FileMode.Create)))
                        {
                            o.Write(offset);
                        }
                    }
                }
            }
        }

        public static IEnumerable<string> GetLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                yield return reader.ReadLine();
            }
        }
    }
}
