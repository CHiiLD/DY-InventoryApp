namespace R54IN0.WPF
{
    public class SQLInsertEventArgs
    {
        private IID _iID;

        public SQLInsertEventArgs(IID iID)
        {
            _iID = iID;
        }

        public IID IID
        {
            get
            {
                return _iID;
            }
        }
    }
}