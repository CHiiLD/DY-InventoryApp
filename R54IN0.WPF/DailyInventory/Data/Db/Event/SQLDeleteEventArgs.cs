using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    public class SQLDeleteEventArgs
    {
        private List<string> _ids;
        private Type _type;

        public SQLDeleteEventArgs(Type type, List<string> ids)
        {
            _type = type;
            _ids = ids;
        }

        public SQLDeleteEventArgs(Type type, string id)
        {
            _type = type;
            _ids = new List<string>() { id };
        }

        public Type Type
        {
            get
            {
                return _type;
            }
        }

        public List<string> IDList
        {
            get
            {
                return _ids;
            }
        }
    }
}