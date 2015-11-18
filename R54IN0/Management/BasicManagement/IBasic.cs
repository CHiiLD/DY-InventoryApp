using System;

namespace DY.Inven
{
    public interface IBasic : IUUID
    {
        string Name { get; set; }
        bool IsDeleted { get; set; }
    }
}