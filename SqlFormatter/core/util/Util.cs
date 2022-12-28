using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core.util
{
    public class Util
    {

        public static List<T> NullToEmpty<T>(List<T> list)
        {
            if (list == null)
            {
                return new List<T>();
            }
            return list;
        }

        public static string TrimSpacesEnd(string s)
        {
            int endIndex = s.Length;
            char[] chars = s.ToCharArray();
            while (endIndex > 0 && (chars[endIndex - 1] == ' ' || chars[endIndex - 1] == '\t')) {
                endIndex--;
            }
            return new string(chars, 0, endIndex);
            // return s.replaceAll("[ \t]+$", "");
        }

        public static R FirstNotnull<R>(params Func<R>[] suppliers)
        { 
            foreach (Func<R>supplier in suppliers)
            {
                R ret = supplier.Invoke();
                if (ret != null)
                    return ret;
            }
            return default;
        }

        public static string Repeat(string s, int n)
        {
            return Enumerable.Repeat(s, n).Aggregate((x, y) => x + y);
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
