namespace R54IN0.WPF
{
    public class SQLInsertEventArgs
    {
        private object _obj;

        public SQLInsertEventArgs(object obj)
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