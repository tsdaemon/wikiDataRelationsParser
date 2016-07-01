using System.Collections.Generic;
using MongoDB.Bson;

namespace Core.Model
{
    public class Triplet
    {
        public ObjectId Id { get; set; }
        public string Property { get; set; }
        public string SectionId { get; set; }
        public string Category { get; set; }
        public Dictionary<string, string> WikiResult { get; set; }
        public string ObjectWikiName { get; set; }
        public string SubjectWikiName { get; set; }

        public List<Position> Positions { get; set; }
        public List<AnotherArticlePosition> ArticlePositions { get; set; }

        public Triplet()
        {
            Positions = new List<Position>();
            ArticlePositions = new List<AnotherArticlePosition>();
        }
    }

    public class Position
    {
        public string Anchor { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as Position;
            return o.Start == Start &&
                   o.End == End &&
                   o.Anchor == Anchor;
        }
    }

    public class AnotherArticlePosition
    {
        public string ArticleTitle { get; set; }
        public long ArticleId { get; set; }
        public Position ObjectPosition { get; set; }
        public Position SubjectPosition { get; set; }
        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            var o = obj as AnotherArticlePosition;
            return o.ObjectPosition.Equals(ObjectPosition) &&
                   o.SubjectPosition.Equals(SubjectPosition) &&
                   o.ArticleId == ArticleId;
        }
    }
}
