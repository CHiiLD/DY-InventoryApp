using System.Collections.Generic;

namespace R54IN0
{
    public class CollectionViewModelObserverSubject
    {
        private static CollectionViewModelObserverSubject _thiz;
        private List<ICollectionViewModelObserver> _observers;

        private CollectionViewModelObserverSubject()
        {
            _observers = new List<ICollectionViewModelObserver>();
        }

        public static CollectionViewModelObserverSubject GetInstance()
        {
            if (_thiz == null)
                _thiz = new CollectionViewModelObserverSubject();
            return _thiz;
        }

        public static void Destory()
        {
            if (_thiz != null)
                _thiz = null;
        }

        public void Attach(ICollectionViewModelObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(ICollectionViewModelObserver observer)
        {
            if (_observers.Contains(observer))
                _observers.Remove(observer);
        }

        public void NotifyNewItemAdded(object item)
        {
            foreach (var observer in _observers)
                observer.UpdateNewItem(item);
        }

        public void NotifyItemDeleted(object item)
        {
            foreach (var observer in _observers)
                observer.UpdateDelItem(item);
        }
    }
}