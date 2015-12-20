using System;
using System.Collections.Generic;

namespace R54IN0
{
    public class ViewModelObserverSubject
    {
        static ViewModelObserverSubject _thiz;
        List<IViewModelObserver> _observers;

        ViewModelObserverSubject()
        {
            _observers = new List<IViewModelObserver>();
        }

        public static ViewModelObserverSubject GetInstance()
        {
            if (_thiz == null)
                _thiz = new ViewModelObserverSubject();
            return _thiz;
        }

        public static void Distory()
        {
            if (_thiz != null)
            {
                //_thiz._observers = null;
                _thiz = null;
            }
        }

        public void Attach(IViewModelObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Detach(IViewModelObserver observer)
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