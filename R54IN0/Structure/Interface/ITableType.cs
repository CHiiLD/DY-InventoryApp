using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IObservable : IID, INotifyPropertyChanged
    {
        bool AllowSave { get; set; }
        void NotifyPropertyChanged(string propertyName);
    }
}