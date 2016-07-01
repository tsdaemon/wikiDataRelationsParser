using System.Collections.Generic;

namespace Core.Model
{
    public class UnitPart
    {
        public string Category { get; set; }
        public string Property { get; set; }
        public string Qualifier { get; set; }

        public long Offset { get; set; }

        public bool Started { get; set; }
        public bool Done { get; set; }
    }

    public class Unit
    {
        public string Id { get; set; }
        public List<UnitPart> Parts { get; set; }
    }
}
