using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    [Flags]
    public enum StockType
    {
        NONE = 0,
        INCOMING = 1 << 0,
        OUTGOING = 1 << 1,
        ALL = INCOMING | OUTGOING
    }
}