using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0
{
    public class ViewModelObserver<T> : IViewModelObserver where T : class
    {
        ViewModelObserverSubject _subject;

        public ViewModelObserver(ViewModelObserverSubject subject)
        {
            subject.Attach(this);
            _subject = subject;
        }

        ~ViewModelObserver()
        {
            _subject.Detach(this);
        }

        public virtual ObservableCollection<T> Items { get; set; }

        public virtual void Add(T item)
        {
            if (_subject != null)
                _subject.NotifyNewItemAdded(item);
        }

        public virtual void Remove(T item)
        {
            if (_subject != null)
                _subject.NotifyItemDeleted(item);
        }

        public virtual void UpdateNewItem(object item)
        {
            if (!Items.Contains(item) && item is T)
                Items.Add(item as T);
        }

        public virtual void UpdateDelItem(object item)
        {
            if (Items.Contains(item) && item is T)
                Items.Remove(item as T);
        }
    }
}
