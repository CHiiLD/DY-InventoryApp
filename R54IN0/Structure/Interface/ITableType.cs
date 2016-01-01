using System.ComponentModel;

namespace R54IN0
{
    public interface IObservable : IID, INotifyPropertyChanged
    {
        bool AllowSave { get; set; }

        void NotifyPropertyChanged(string propertyName);
    }
}