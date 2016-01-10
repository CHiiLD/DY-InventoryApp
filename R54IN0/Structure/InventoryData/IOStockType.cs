using System;

namespace R54IN0
{
    [Flags]
    public enum IOStockType
    {
        NONE = 0,
        INCOMING = 1 << 0,
        OUTGOING = 1 << 1,
        RETURN = 1 << 2,
        ALL = INCOMING | OUTGOING | RETURN
    }
}