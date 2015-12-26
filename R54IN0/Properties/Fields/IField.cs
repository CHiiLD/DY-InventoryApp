using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public interface IField : IUUID
    {
        string Name { get; set; }
        bool IsDeleted { get; set; }
    }
}