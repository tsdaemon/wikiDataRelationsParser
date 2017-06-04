using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    public class TripletTrain
    {
        public string Id { get; set; }

        public string Predicate { get; set; }

        public string Object { get; set; }

        public string ObjectAnchor { get; set; }

        public string Subject { get; set; }

        public string SubjectAnchor { get; set; }

        public string WikipediaLink { get; set; }

        public string WikipediaTitle { get; set; }

        public string Text { get; set; }

        public RelationLabel Label { get; set; }
    }

    public enum RelationLabel
    {
        Unknown,
        NoRelation,
        WeakRelation,
        Relation
    }
}
