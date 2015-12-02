using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace R54IN0
{
    public class SortedObservableCollection<T> : ObservableCollection<T> where T : IComparable
    {
        int _low;

        public SortedObservableCollection() : base()
        {
        }

        public SortedObservableCollection(IEnumerable<T> items) : base(items.OrderBy(x => x))
        {
        }

        public new T this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public new void Add(T item)
        {
            int index = BinarySearch(item);
            if (index == -1)
                index = _low;
            else
                base.RemoveAt(index);
            base.Insert(index, item);
        }

        public new bool Contains(T item)
        {
            return BinarySearch(item) == -1 ? false : true;
        }

        public new bool Remove(T item)
        {
            int index = BinarySearch(item);
            if (index == -1)
                return false;
            else
                base.RemoveAt(index);
            return true;
        }

        protected int BinarySearch(T item)
        {
            _low = 0;
            int high = Count - 1;
            int mid = 0;
            T t;

            while (_low <= high)
            {
                mid = (_low + high) / 2;
                t = Items.ElementAt(mid);
                if (t.CompareTo(item) > 0)
                    high = mid - 1;
                else if (t.CompareTo(item) < 0)
                    _low = mid + 1;
                else
                    return mid;
            }
            return -1;
        }

        public new void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public new void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public new void Move(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException();
        }
    }
}
