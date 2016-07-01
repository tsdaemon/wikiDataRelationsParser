using Core.Model;
using LINQtoCSV;

namespace Core.Wikifile
{
    public class PositionLine
    {
        [CsvColumn(FieldIndex = 1, Name = "title")]
        public string WikiTitle { get; set; }

        [CsvColumn(FieldIndex = 2, Name = "pageid")]
        public long PageId { get; set; }

        [CsvColumn(FieldIndex = 3, Name = "link")]
        public string EntityName { get; set; }

        [CsvColumn(FieldIndex = 4, Name = "anchor")]
        public string Anchor { get; set; }

        [CsvColumn(FieldIndex = 5, Name = "start")]
        public int Start { get; set; }

        [CsvColumn(FieldIndex = 6, Name = "end")]
        public int End { get; set; }

        public Position ToPosition()
        {
            return new Position
            {
                Anchor = Anchor ?? EntityName,
                End = End,
                Start = Start
            };
        }
    }
}
