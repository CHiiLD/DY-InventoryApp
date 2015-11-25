using System;

namespace R54IN0
{
    public interface IBasic : IUUID
    {
        string Name { get; set; }
        bool IsDeleted { get; set; }
    }
}