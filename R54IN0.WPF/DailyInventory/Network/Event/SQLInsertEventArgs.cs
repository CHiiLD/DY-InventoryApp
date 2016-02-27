namespace R54IN0.WPF
{
    public class SQLInsertEventArgs
    {
        private IID _format;

        public SQLInsertEventArgs(IID format)
        {
            _format = format;
        }

        public IID Format
        {
            get
            {
                return _format;
            }
        }
    }
}