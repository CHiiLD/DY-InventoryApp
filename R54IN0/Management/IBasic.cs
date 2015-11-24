using System;

namespace R54IN0.Lib
{
    public interface IBasic : IUUID
    {
        string Name { get; set; }
        bool IsDeleted { get; set; }
    }
}