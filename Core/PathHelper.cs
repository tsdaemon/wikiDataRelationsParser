using System.IO;
using System.Text.RegularExpressions;

namespace Core
{
    public static class PathHelper
    {
        public static string EncodeFilePathPart(string part)
        {
            return Regex.Replace(part, "[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]", "_");
        }
    }
}
