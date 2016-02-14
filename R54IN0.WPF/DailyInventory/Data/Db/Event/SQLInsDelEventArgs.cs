namespace R54IN0
{
    public class SQLInsDelEventArgs
    {
        private object _obj;

        public SQLInsDelEventArgs(object obj)
        {
            _obj = obj;
        }

        public object Data
        {
            get
            {
                return _obj;
            }
        }
    }
}