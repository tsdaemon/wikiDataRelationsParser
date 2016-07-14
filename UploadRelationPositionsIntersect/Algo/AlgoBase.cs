using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.Model;
using Core.Service;
using Core.Wikifile;
using LINQtoCSV;
using MongoDB.Driver;

namespace UploadRelationPositionsIntersect.Algo
{
    public abstract class AlgoBase
    {
        protected AsyncSaver _saver;
        protected IWikidumpReader _wikiReader;
        protected IMongoCollection<Triplet> _triplets;
        protected IEnumerable<PositionLine> _positions;
        protected int _positionsSet;

        protected AlgoBase(AsyncSaver saver, 
            IWikidumpReader wikiReader, 
            IMongoCollection<Triplet> triplets,
            string positionsPath)
        {
            _saver = saver;
            _wikiReader = wikiReader;
            _triplets = triplets;
            _positions = PrepareCsvReader(positionsPath);
        }

        public void Process(int offset)
        {
            var linesDone = 0;
            _positionsSet = 0;
            foreach (var g in _positions.Skip(offset).GroupBySequentually(l => l.PageId))
            {
                var values = g.ToArray();
                for (var i = 0; i < values.Length; i++)
                {
                    for (var j = i + 1; j < values.Length; j++)
                    {
                        var entity1 = values[i];
                        var entity2 = values[j];
                        if (entity1.EntityName != entity2.EntityName)
                        {
                            ProcessPair(entity1, entity2);
                        }
                    }
                }

                linesDone += values.Length;
                ProcessOffset(linesDone, _positionsSet);
            }
        }

        public delegate void Processed(AlgoProcessedEvent ev);

        public event Processed OnProcessed;

        protected abstract void ProcessPair(PositionLine entity1, PositionLine entity2);

        protected void ProcessOffset(int done, int positions)
        {
            var ev = new AlgoProcessedEvent
            {
                LinesDone = done,
                PositionsSet = positions
            };
            if (OnProcessed != null)
            {
                OnProcessed(ev);
            }
        }

        private IEnumerable<PositionLine> PrepareCsvReader(string file)
        {
            var inputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            var cc = new CsvContext();
            return cc.Read<PositionLine>(file, inputFileDescription);
        }

        protected void ProcessTriplet(Triplet t,
            PositionLine object_,
            PositionLine subject,
            IWikidumpReader reader)
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

            _saver.Save(t, position);
        }
    }
}
