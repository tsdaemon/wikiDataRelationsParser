using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace UploadRelationPositions
{
    class PositionLine
    {
        [CsvColumn(FieldIndex = 1)]
        public string ObjectWiki { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public string SubjectWiki { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public string Anchor { get; set; }
        [CsvColumn(FieldIndex = 5)]
        public int Start { get; set; }
        [CsvColumn(FieldIndex = 6)]
        public int End { get; set; }
    }
}
