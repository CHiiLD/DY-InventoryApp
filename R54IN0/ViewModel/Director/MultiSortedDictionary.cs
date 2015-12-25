using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class MultiSortedDictionary<Key, Value>
    {
        private SortedDictionary<Key, List<Value>> _dic = null;

        public MultiSortedDictionary()
        {
            _dic = new SortedDictionary<Key, List<Value>>();
        }

        public MultiSortedDictionary(IComparer<Key> comparer)
        {
            _dic = new SortedDictionary<Key, List<Value>>(comparer);
        }

        public void Add(Key key, Value value)
        {
            List<Value> list = null;

            if (_dic.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<Value>();
                list.Add(value);
                _dic.Add(key, list);
            }
        }

        public bool ContainsKey(Key key)
        {
            return _dic.ContainsKey(key);
        }

        public bool Remove(Key key, Value value)
        {
            if (!ContainsKey(key))
                return false;
            return this[key].Remove(value);
        }

        public List<Value> this[Key key]
        {
            get
            {
                List<Value> list = null;
                if (!_dic.TryGetValue(key, out list))
                {
                    list = new List<Value>();
                    _dic.Add(key, list);
                }
                return list;
            }
        }

        public IEnumerable<Key> keys
        {
            get
            {
                return _dic.Keys;
            }
        }
    }
}
