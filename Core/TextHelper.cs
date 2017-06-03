namespace Core
{
    public static class TextHelper
    {
        public static string ExtractTextWithSentenceWindow(string text, int start, int end, out int newStart, out int newEnd)
        {
            newStart = start;
            if (newStart >= text.Length)
            {
                newStart = text.Length - 1;
            }
            while (newStart > 1)
            {
                var twoChar = text.Substring(newStart - 2, 2);
                if (twoChar == ". " || twoChar[1] == '\n') break;

                newStart--;
            }
            if (newStart <= 1) newStart = 0;

            newEnd = end;
            while (newEnd < text.Length - 2)
            {
                var twoChar = text.Substring(newEnd, 2);
                if (twoChar == ". " || twoChar[0] == '\n') break;

                newEnd++;
            }
            if (newEnd >= text.Length - 2) newEnd = text.Length - 1;


            return text.Substring(newStart, newEnd - newStart).Trim();
        }
    }
}
