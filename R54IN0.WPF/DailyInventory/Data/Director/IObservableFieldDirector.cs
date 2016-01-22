using System.Collections.Generic;

namespace R54IN0
{
    public interface IObservableFieldDirector
    {
        void AddObservableField<T>(IObservableField observableField) where T : class, IField, new();
        IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new();
        void RemoveObservableField<T>(IObservableField observableField) where T : class, IField, new();
        Observable<T> SearchObservableField<T>(string id) where T : class, IField, new();
    }
}