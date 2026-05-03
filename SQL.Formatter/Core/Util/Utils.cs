using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.Core.Util
{
    public class Utils
    {
        public static List<T> NullToEmpty<T>(List<T> list)
        {
            return list ?? new List<T>();
        }

        public static List<T> Concat<T>(List<T> l1, List<T> l2)
        {
            return l1.Concat(l2).ToList();
        }

        public static JSLikeList<string> SortByLengthDesc(JSLikeList<string> strings)
        {
            return new JSLikeList<string>(
                strings.ToList().OrderByDescending(s => s.Length).ToList());
        }
    }
}
