using System.Collections.Generic;

namespace SQL.Formatter.Core
{
    public abstract class Params
    {
        public static readonly Params Empty = new Empty();

        public abstract bool IsEmpty();

        public abstract object Get();

        public abstract object GetByName(string key);

        public static Params Of<T>(Dictionary<string, T> parameters)
        {
            return new NamedParams<T>(parameters);
        }

        public static Params Of<T>(List<T> parameters)
        {
            return new IndexedParams<T>(new Queue<T>(parameters));
        }

        public object Get(Token token)
        {
            if (IsEmpty())
            {
                return token.Value;
            }

            if (!(token.Key == null || string.IsNullOrEmpty(token.Key)))
            {
                return GetByName(token.Key);
            }

            return Get();
        }
    }

    internal class NamedParams<T> : Params
    {
        private readonly Dictionary<string, T> _parameters;

        public NamedParams(Dictionary<string, T> parameters)
        {
            _parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return _parameters.Count == 0;
        }

        public override object Get()
        {
            return null;
        }

        public override object GetByName(string key)
        {
            return _parameters[key];
        }

        public override string ToString()
        {
            return _parameters.ToString();
        }
    }

    internal class IndexedParams<T> : Params
    {
        private readonly Queue<T> _parameters;

        public IndexedParams(Queue<T> parameters)
        {
            _parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return _parameters.Count == 0;
        }

        public override object Get()
        {
            return _parameters.Dequeue();
        }

        public override object GetByName(string key)
        {
            return null;
        }

        public override string ToString()
        {
            return _parameters.ToString();
        }
    }

    internal class Empty : Params
    {
        public override bool IsEmpty()
        {
            return true;
        }

        public override object Get()
        {
            return null;
        }

        public override object GetByName(string key)
        {
            return null;
        }

        public override string ToString()
        {
            return "[]";
        }
    }
}
