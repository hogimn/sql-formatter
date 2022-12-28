﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace VerticalBlank.SqlFormatter.core
{
    /** Handles placeholder replacement with given params. */
    public abstract class Params
    {
        public static readonly Params EMPTY = new Empty();

        public abstract bool IsEmpty();

        public abstract object Get();

        public abstract object GetByName(string key);

        /**
         * @param params query param
         */
        public static Params Of<T>(Dictionary<string, T> parameters)
        {
            return new NamedParams<T>(parameters);
        }

        /**
         * @param params query param
         */
        public static Params Of<T>(List<T> parameters)
        {
            return new IndexedParams<T>(new Queue<T>(parameters));
        }

        /**
         * Returns param value that matches given placeholder with param key.
         *
         * @param token token.key Placeholder key token.value Placeholder value
         * @return param or token.value when params are missing
         */
        public object Get(Token token)
        {
            if (IsEmpty())
            {
                return token.value;
            }
            if (!(token.key == null || string.IsNullOrEmpty(token.key)))
            {
                return GetByName(token.key);
            }
            return Get();
        }
    }

    class NamedParams<T> : Params
    {
        private readonly Dictionary<string, T> parameters;

        public NamedParams(Dictionary<string, T> parameters)
        {
            this.parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return parameters.Count == 0;
        }

        public override object Get()
        {
            return null;
        }

        public override object GetByName(string key)
        {
            return parameters[key];
        }

        public override string ToString()
        {
            return parameters.ToString();
        }
    }

    class IndexedParams<T> : Params
    {
        private readonly Queue<T> parameters;

        public IndexedParams(Queue<T> parameters)
        {
            this.parameters = parameters;
        }

        public override bool IsEmpty()
        {
            return parameters.Count == 0;
        }

        public override object Get()
        {
            return parameters.Dequeue();
        }

        public override object GetByName(string key)
        {
            return null;
        }

        public override string ToString()
        {
            return parameters.ToString();
        }
    }


    class Empty : Params
    {
        public Empty() { }

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
