﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQtoCSV;

namespace UploadRelationPositions
{
    class PositionLine
    {
        [CsvColumn(FieldIndex = 1, Name = "title")]
        public string ObjectWiki { get; set; }

        [CsvColumn(FieldIndex = 2, Name = "pageid")]
        public long PageId { get; set; }

        [CsvColumn(FieldIndex = 3, Name = "link")]
        public string SubjectWiki { get; set; }

        [CsvColumn(FieldIndex = 4, Name = "anchor")]
        public string Anchor { get; set; }

        [CsvColumn(FieldIndex = 5, Name = "start")]
        public int Start { get; set; }

        [CsvColumn(FieldIndex = 6, Name = "end")]
        public int End { get; set; }
    }
}
