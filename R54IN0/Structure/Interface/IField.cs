using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public interface IField : IID , IName
    {
        bool IsDeleted { get; set; }
    }
}