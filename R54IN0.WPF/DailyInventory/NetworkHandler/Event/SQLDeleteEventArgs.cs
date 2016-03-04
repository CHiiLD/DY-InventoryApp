using System;

namespace R54IN0.WPF
{
    public class SQLDeleteEventArgs
    {
        private Type _type;
        private string _id;

        public SQLDeleteEventArgs(Type type, string id)
        {
            _id = id;
            _type = type;
        }

        public Type Type
        {
            get
            {
                return _type;
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