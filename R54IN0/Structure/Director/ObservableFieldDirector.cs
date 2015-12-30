using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class ObservableFieldDirector
    {
        static ObservableFieldDirector _thiz;

        Dictionary<Type, SortedDictionary<string, object>> _dic;

        ObservableFieldDirector()
        {
            _dic = new Dictionary<Type, SortedDictionary<string, object>>();
        }

        public static ObservableFieldDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableFieldDirector();
            return _thiz;
        }

        public Observable<T> Search<T>(string id) where T : class, IField, new()
        {
            if (string.IsNullOrEmpty(id))
                return null;
            Type type = typeof(T);
            if (!_dic.ContainsKey(type))
            {
                _dic[type] = new SortedDictionary<string, object>();
                T[] ts = null;
                using (var db = LexDb.GetDbInstance())
                    ts = db.LoadAll<T>();
                foreach (var t in ts)
                    _dic[type].Add(t.ID, new Observable<T>(t));
            }
            return _dic[type].ContainsKey(id) ? _dic[type][id] as Observable<T> : null;
        }
    }
}
