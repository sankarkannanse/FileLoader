using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoader
{
   public class Utils
    {
        internal static bool ToBoolean(string c)
        {
            return Convert.ToBoolean(Enum.Parse(typeof(BooleanAliases), c.ToUpper()));
        }

        enum BooleanAliases
        {
            Y = 1,
            N = 0
        }
    }
}
