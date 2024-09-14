using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SQL.Formatter.Core.Util
{
    public class JSLikeList<T> : IEnumerable
    {
        private readonly List<T> _tList;

        public JSLikeList(List<T> tList)
        {
            _tList = tList ?? new List<T>();
        }

        public List<T> ToList()
        {
            return _tList;
        }

        public JSLikeList<R> Map<R>(Func<T, R> mapper)
        {
            return new JSLikeList<R>(
                _tList
                .Select(mapper.Invoke)
                .ToList());
        }

        public string Join(string delimiter)
        {
            return string.Join(delimiter, _tList);
        }

        public JSLikeList<T> With(List<T> other)
        {
            return new JSLikeList<T>(
                _tList
                .Concat(other)
                .ToList());
        }

        public string Join()
        {
            return Join(",");
        }

        public bool IsEmpty()
        {
            return _tList == null || _tList.Count == 0;
        }

        public T Get(int index)
        {
            if (index < 0 || _tList.Count <= index)
            {
                return default;
            }

            return _tList.ElementAt(index);
        }

        public IEnumerator GetEnumerator()
        {
            return _tList.GetEnumerator();
        }

        public int Size()
        {
            return _tList.Count;
        }
    }
}
