using System;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0
{
    public class ObservableFieldDirector
    {
        private static ObservableFieldDirector _thiz;

        private Dictionary<Type, SortedDictionary<string, object>> _dic;

        private ObservableFieldDirector()
        {
            _dic = new Dictionary<Type, SortedDictionary<string, object>>();
        }

        public static ObservableFieldDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableFieldDirector();
            return _thiz;
        }

        public static void Distory()
        {
            if (_thiz != null)
                _thiz._dic = null;
            _thiz = null;
        }

        public Observable<T> Search<T>(string id) where T : class, IField, new()
        {
            if (string.IsNullOrEmpty(id))
                return null;
            Type type = typeof(T);
            if (!_dic.ContainsKey(type))
                AddNewTypeCollection<T>();
            return _dic[type].ContainsKey(id) ? _dic[type][id] as Observable<T> : null;
        }

        public List<Observable<T>> CreateList<T>() where T : class, IField, new()
        {
            Type type = typeof(T);
            if (!_dic.ContainsKey(type))
                AddNewTypeCollection<T>();
            return _dic[type].Values.Cast<Observable<T>>().ToList();
        }
        private void AddNewTypeCollection<T>() where T : class, IField, new()
        {
            Type type = typeof(T);
            _dic[type] = new SortedDictionary<string, object>();
            T[] ts = null;
            using (var db = LexDb.GetDbInstance())
                ts = db.LoadAll<T>();
            foreach (var t in ts)
                _dic[type].Add(t.ID, new Observable<T>(t));
        }

        public void Add<T>(object field) where T : class, IField, new()
        {
            if (field is IObservableField)
            {
                Type type = typeof(T);
                if (!_dic.ContainsKey(type))
                    AddNewTypeCollection<T>();
                IObservableField obserableField = field as IObservableField;
                if (!_dic[type].ContainsKey(obserableField.ID))
                    _dic[type].Add(obserableField.ID, obserableField);
            }
        }

        public void Remove<T>(string id) where T : class, IField, new()
        {
            Type type = typeof(T);
            if (!_dic.ContainsKey(type))
                AddNewTypeCollection<T>();
            if (_dic[type].ContainsKey(id))
                _dic[type].Remove(id);
        }
    }
}