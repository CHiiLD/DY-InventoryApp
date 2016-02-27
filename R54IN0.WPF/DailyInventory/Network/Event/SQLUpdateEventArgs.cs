using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    public class SQLUpdateEventArgs
    {
        private IID _iid;

        public SQLUpdateEventArgs(IID format)
        {
            _iid = format;
        }

        public IID Format
        {
            get
            {
                return _iid;
            }
        }
    }
}