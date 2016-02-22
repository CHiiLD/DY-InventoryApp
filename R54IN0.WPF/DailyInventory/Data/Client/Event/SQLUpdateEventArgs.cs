using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    public class SQLUpdateEventArgs
    {
        private IDictionary<string, object> _content;
        private string _id;
        private Type _type;

        public SQLUpdateEventArgs(Type type, string id, IDictionary<string, object> content)
        {
            _type = type;
            _id = id;
            _content = content;
        }

        public SQLUpdateEventArgs(Type type, string id, string propertyName, object value)
        {
            _type = type;
            _id = id;
            _content = new Dictionary<string, object>();
            _content.Add(propertyName, value);
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public IDictionary<string, object> Content
        {
            get
            {
                return _content;
            }
        }

        public string ID
        {
            get
            {
                return _id;
            }
        }
    }
}