using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class WFieldSortedObservableCollection<T, IFieldType> : SortedObservableCollection<T>
        where T : WField<IFieldType>
        where IFieldType : class, IField, new()
    {
        int _low;

        public WFieldSortedObservableCollection() : base()
        {
        }

        public WFieldSortedObservableCollection(IEnumerable<T> items) : base(items.OrderBy(x => x))
        {
        }

        public T BinarySearchAsUUID(string uuid)
        {
            _low = 0;
            int high = Count - 1;
            int mid = 0;
            string filed_uuid;
            bool isContains = false;
            while (_low <= high)
            {
                mid = (_low + high) / 2;
                filed_uuid = Items.ElementAt(mid).Field.UUID;
                if (filed_uuid.CompareTo(uuid) > 0)
                    high = mid - 1;
                else if (filed_uuid.CompareTo(uuid) < 0)
                    _low = mid + 1;
                else
                {
                    isContains = true;
                    break;
                }
            }
            return isContains ? Items.ElementAt(mid) : null;
        }
    }
}
