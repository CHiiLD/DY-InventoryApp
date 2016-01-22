using System.Collections.Generic;

namespace R54IN0
{
    public interface IObservableFieldDirector
    {
        void AddObservableField(IObservableField observableField);
        IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new();
        void RemoveObservableField(IObservableField observableField);
        Observable<T> SearchObservableField<T>(string id) where T : class, IField, new();
    }
}