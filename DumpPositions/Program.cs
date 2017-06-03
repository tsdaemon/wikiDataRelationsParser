using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using LINQtoCSV;
using MongoDB.Driver;

namespace DumpPositions
{
    class Program
    {
        const string PositionsFilePath = "D:\\DRIVE\\ukr-ner\\positions.csv";
        static void Main(string[] args)
        {
            var db = new MongoClient("mongodb://127.0.0.1:27017/").GetDatabase("wikidata");
            var triplets = db.GetCollection<Triplet>("triplet");
            var tt = getEnumerator(triplets);

            var inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            var cc = new CsvContext();
            cc.Write(tt, PositionsFilePath);
        }

        private static IEnumerable<TripletLine> getEnumerator(IMongoCollection<Triplet> triplets)
        {
            return from t in get(triplets) from p in t.ArticlePositions select TripletLine.FromTripletAndPosition(t, p);
        }

        private static IEnumerable<Triplet> get(IMongoCollection<Triplet> triplets)
        {
            var fields = Builders<Triplet>.Projection
                .Exclude(t => t.ArticlePositions[0].Text);
            return triplets
                .Find(r => r.ArticlePositions.Any())
                .Project(fields)
                .As<Triplet>()
                .ToEnumerable();

        }

        public class TripletLine
        {
            public static TripletLine FromTripletAndPosition(Triplet t, AnotherArticlePosition a)
            {
                return new TripletLine()
                {
                    Object = t.ObjectWikiName,
                    Property = t.Property,
                    Subject = t.SubjectWikiName,
                    ArticleTitle = a.ArticleTitle,
                    ArticleId = a.ArticleId,
                    Start = a.Start,
                    End = a.End,
                    Distance = a.Distance
                };
            }

            [CsvColumn(FieldIndex = 8, Name = "Distance")]
            public int Distance { get; set; }

            [CsvColumn(FieldIndex = 7, Name = "End")]
            public int End { get; set; }

            [CsvColumn(FieldIndex = 6, Name = "Start")]
            public int Start { get; set; }

            [CsvColumn(FieldIndex = 5, Name = "ArticleId")]
            public long ArticleId { get; set; }

            [CsvColumn(FieldIndex = 3, Name = "Property")]
            public string Property { get; set; }

            [CsvColumn(FieldIndex = 4, Name = "ArticleTitle")]
            public string ArticleTitle { get; set; }

            [CsvColumn(FieldIndex = 1, Name = "Subject")]
            public string Subject { get; set; }

            [CsvColumn(FieldIndex = 2, Name = "Object")]
            public string Object { get; set; }
        }
    }
}
