using System.Linq;
using System.Text.RegularExpressions;

namespace Core
{
    public static class TextHelper
    {
        static readonly string[] abbrs = @"ім.
о.
вул.
просп.
бул.
пров.
пл.
г.
р.
див.
п.
с.
м.".Trim().Split();

        static readonly char[] endings = { '.', '!', '?', '…', '»' };

        private static Regex tagsBeforeEnd = new Regex(@"\.\<[\w]+\>\{\{[\w\s\|\=\.\:\/\-]+\}\}\<\/[\w]+\>", RegexOptions.Compiled);

        public static string ExtractTextWithSentenceWindow(string text, int start, int end, out int newStart, out int newEnd)
        {
            var checkTags = tagsBeforeEnd.IsMatch(text);

            newStart = FindSentenceStart(text, start, checkTags);

            newEnd = FindSentenceEnd(text, end, checkTags);

            return text.Substring(newStart, newEnd - newStart).Trim();
        }

        private static int FindSentenceStart(string text, int index, bool checkTags)
        {
            if (index >= text.Length)
            {
                index = text.Length - 1;
            }

            var tokenEnd = index;
            var returnIndex = index;

            var tokens = new string[2];
            var inToken = text[index] != ' ';
            var inTag = false;

            while (index > 0)
            {
                index--;
                var tokenStart = index + 1;
                var subs = text.Substring(tokenStart, tokenEnd - tokenStart);

                // check if we inside of the tag
                if (checkTags)
                    inTag = CheckInTag(subs, inTag);

                if (!inTag)
                {
                    var currentChar = text[index];

                    // special case for the line break
                    if (currentChar == '\n') return index + 1;

                    if (currentChar == ' ' && inToken) // token end found
                    {
                        
                        // move first token to the second place
                        tokens[1] = tokens[0];
                        // put substring to the first place
                        tokens[0] = subs;
                        // check both tokens exists and contains sentence end
                        if (CheckSentenceEnd(tokens, checkTags)) return returnIndex;

                        // set token flag
                        inToken = false;
                        // set return index to current token start
                        returnIndex = index + 1;
                    }

                    if (currentChar != ' ' && !inToken) // token start found
                    {
                        // set token flag
                        inToken = true;
                        tokenEnd = index + 1;
                    }
                }
            }
            return index;
        }

        private static int FindSentenceEnd(string text, int index, bool checkTags)
        {
            var tokenStart = index;
            var tokens = new string[2];
            var returnIndex = index;

            if (index == text.Length) return index;

            var inToken = text[index] != ' ';
            var inTag = false;

            while (index < text.Length-1)
            {
                index++;
                var tokenEnd = index;
                var subs = text.Substring(tokenStart, tokenEnd - tokenStart);

                // check if we inside of the tag
                if(checkTags)
                    inTag = CheckInTag(subs, inTag);

                if (!inTag)
                {

                    var currentChar = text[index];

                    // special case for the line break
                    if (currentChar == '\n') return index;

                    if (currentChar == ' ' && inToken) // token end found
                    {
                        // move first token to second place
                        tokens[0] = tokens[1];
                        // put substring to the first place

                        tokens[1] = text.Substring(tokenStart, tokenEnd - tokenStart);
                        // check both tokens exists and contains sentence end
                        if (CheckSentenceEnd(tokens, checkTags)) return returnIndex;

                        // set token flag
                        inToken = false;
                        // set return index to current token start
                        returnIndex = index;
                    }

                    if (currentChar != ' ' && !inToken) // token start found
                    {
                        // set token flag
                        inToken = true;
                        tokenStart = index;
                    }
                }
            }
            return index;
        }

        private static bool CheckInTag(string subs, bool inTag)
        {
            // check if we inside of the tag
            if (!inTag && Regex.IsMatch(subs, @"\<\/?[\w]+\>"))
                return true;
            if (inTag && Regex.IsMatch(subs, @"\<[\w]+\>.+\<\/\w+>"))
                return false;
            return inTag;
        }

        /// <summary>
        /// Check whether two sentence tokens contains sentence end
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static bool CheckSentenceEnd(string[] tokens, bool checkTags)
        {
            if (string.IsNullOrEmpty(tokens[0]) || string.IsNullOrEmpty(tokens[1])) return false;

            var token1 = tokens[0];
            var token2 = tokens[1];

            // special case for ref tags
            if (checkTags && tagsBeforeEnd.IsMatch(tokens[0])) return true;

            if (endings.Contains(token1[token1.Length - 1]))
            {
                var stopToken = Regex.Match(token1, @"[.!?…»]").Value;

                if (token2[0].IsUpper()
                    && !stopToken[0].IsUpper()
                    && !(token1[token1.Length - 1] != '.' ||
                         stopToken[0] == '(' ||
                         abbrs.Contains(token1)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
