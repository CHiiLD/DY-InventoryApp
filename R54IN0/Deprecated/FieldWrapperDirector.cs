using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace R54IN0
{
    public class FieldWrapperDirector
    {
        private static FieldWrapperDirector _thiz;
        private Dictionary<Type, List<IObservableField>> _map;
        private SortedDictionary<string, IObservableField> _idDic;

        private FieldWrapperDirector()
        {
            _map = new Dictionary<Type, List<IObservableField>>();
            _idDic = new SortedDictionary<string, IObservableField>();
        }

        public static void Distroy()
        {
            if (_thiz != null)
            {
                _thiz._map = null;
                _thiz._idDic = null;
            }
            _thiz = null;
        }

        public static FieldWrapperDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FieldWrapperDirector();
            return _thiz;
        }

        public void Add<FieldT>(IObservableField item) where FieldT : class, IField
        {
            if (_map.ContainsKey(typeof(FieldT)))
            {
                item.Field.Save<FieldT>();
                _map[typeof(FieldT)].Add(item);
                _idDic.Add(item.Field.ID, item);
            }
        }

        public bool Contains<FieldT>(IObservableField item) where FieldT : class, IField
        {
#if DEBUG
            Debug.Assert(_map[typeof(FieldT)].Contains(item) == _idDic.ContainsKey(item.Field.ID));
#endif
            if (_map.ContainsKey(typeof(FieldT)))
                return _map[typeof(FieldT)].Contains(item);
            else
                return false;
        }

        public bool Remove<FieldT>(IObservableField item) where FieldT : class, IField
        {
            if (_map.ContainsKey(typeof(FieldT)))
            {
                item.Field.Delete<FieldT>();
                _idDic.Remove(item.Field.ID);
                return _map[typeof(FieldT)].Remove(item);
            }
            else
            {
                return false;
            }
        }

        public ObservableCollection<IObservableField> CreateCollection<FieldT>() where FieldT : class, IField, new()
        {
            Type type = typeof(FieldT);
            return CreateCollection<FieldT, IObservableField>();
        }

        public ObservableCollection<WrapperT> CreateCollection<FieldT, WrapperT>()
            where FieldT : class, IField, new()
            where WrapperT : class, IObservableField
        {
            Type type = typeof(FieldT);
            if (!_map.ContainsKey(type))
                Load<FieldT, WrapperT>();
            return new ObservableCollection<WrapperT>(_map[type].OfType<WrapperT>());
        }

        public WrapperT BinSearch<FieldT, WrapperT>(string ID) where FieldT : class, IField, new() where WrapperT : class, IObservableField
        {
            if (ID == null)
                return null;
            if (!_map.ContainsKey(typeof(FieldT)))
                Load<FieldT, WrapperT>();
            if (!_idDic.ContainsKey(ID))
            {
#if DEBUG
                Debug.Assert(false);
#endif
                return null;
            }
            return _idDic[ID] as WrapperT;
        }

        private void Load<FieldT, ObserableT>() where FieldT : class, IField, new() where ObserableT : class, IObservableField
        {
            Type type = typeof(FieldT);
            _map[type] = new List<IObservableField>();
            FieldT[] fields = null;
            using (var db = LexDb.GetDbInstance())
            {
                fields = db.LoadAll<FieldT>();
            }
            foreach (var field in fields)
            {
                IObservableField wrapper = null;
                if (type == typeof(Item))
                    wrapper = new ItemWrapper(field as Item);
                else if (type == typeof(Specification))
                    wrapper = new SpecificationWrapper(field as Specification);
                else if (type == typeof(Client))
                    wrapper = new ClientWrapper(field as Client);
                else
                    wrapper = new Observable<FieldT>(field);
                _map[type].Add(wrapper);
                _idDic.Add(field.ID, wrapper);
            }
        }
    }
}