using System.Collections.Generic;

namespace R54IN0.WPF
{
    public class SQLUpdateEventArgs
    {
        private Dictionary<string, object> _content;
        private object _data;

        public SQLUpdateEventArgs(object data, Dictionary<string, object> content)
        {
            _data = data;
            _content = content;
        }

        public SQLUpdateEventArgs(object data, string propertyName, object value)
        {
            _data = data;
            _content = new Dictionary<string, object>();
            _content.Add(propertyName, value);
        }

        public Dictionary<string, object> UpdateContent
        {
            get
            {
                return _content;
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }
        }
    }
}