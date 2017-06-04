using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class StringUtils
    {
        public static bool IsUpper(this char s)
        {
            return Char.IsUpper(s);
        }
    }
}
