using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace R54IN0
{
    public class FieldWrapperDirector
    {
        static FieldWrapperDirector _thiz;
        Dictionary<Type, List<IFieldWrapper>> _map;
        SortedDictionary<string, IFieldWrapper> _uuidDic;

        FieldWrapperDirector()
        {
            _map = new Dictionary<Type, List<IFieldWrapper>>();
            _uuidDic = new SortedDictionary<string, IFieldWrapper>();
        }

        public static void Distroy()
        {
            if (_thiz != null)
            {
                _thiz._map = null;
                _thiz._uuidDic = null;
            }
            _thiz = null;
        }

        public static FieldWrapperDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FieldWrapperDirector();
            return _thiz;
        }

        public void Add<FieldT>(IFieldWrapper item) where FieldT : class, IField
        {
            if (_map.ContainsKey(typeof(FieldT)))
            {
                item.Field.Save<FieldT>();
                _map[typeof(FieldT)].Add(item);
                _uuidDic.Add(item.Field.UUID, item);
            }
        }

        public bool Contains<FieldT>(IFieldWrapper item) where FieldT : class, IField
        {
#if DEBUG
            Debug.Assert(_map[typeof(FieldT)].Contains(item) == _uuidDic.ContainsKey(item.Field.UUID));
#endif
            if (_map.ContainsKey(typeof(FieldT)))
                return _map[typeof(FieldT)].Contains(item);
            else
                return false;
        }

        public bool Remove<FieldT>(IFieldWrapper item) where FieldT : class, IField
        {
            if (_map.ContainsKey(typeof(FieldT)))
            {
                item.Field.Delete<FieldT>();
                _uuidDic.Remove(item.Field.UUID);
                return _map[typeof(FieldT)].Remove(item);
            }
            else
            {
                return false;
            }
        }

        public ObservableCollection<IFieldWrapper> CreateCollection<FieldT>() where FieldT : class, IField
        {
            Type type = typeof(FieldT);
            return CreateCollection<FieldT, IFieldWrapper>();
        }

        public ObservableCollection<WrapperT> CreateCollection<FieldT, WrapperT>()
            where FieldT : class, IField
            where WrapperT : class, IFieldWrapper
        {
            Type type = typeof(FieldT);
            if (!_map.ContainsKey(type))
                Load<FieldT, WrapperT>();
            return new ObservableCollection<WrapperT>(_map[type].OfType<WrapperT>());
        }

        public WrapperT BinSearch<FieldT, WrapperT>(string uuid) where FieldT : class, IField where WrapperT : class, IFieldWrapper
        {
            if (uuid == null)
                return null;
            if (!_map.ContainsKey(typeof(FieldT)))
                Load<FieldT, WrapperT>();
            if (!_uuidDic.ContainsKey(uuid))
            {
#if DEBUG
                Debug.Assert(false);
#endif
                return null;
            }
            return _uuidDic[uuid] as WrapperT;
        }

        void Load<FieldT, WrapperT>() where FieldT : class, IField where WrapperT : class, IFieldWrapper
        {
            Type type = typeof(FieldT);
            _map[type] = new List<IFieldWrapper>();
            FieldT[] fields = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                fields = db.LoadAll<FieldT>();
            }
            foreach (var field in fields)
            {
                IFieldWrapper wrapper = null;
                if (type == typeof(Item))
                    wrapper = new ItemWrapper(field as Item);
                else if (type == typeof(Specification))
                    wrapper = new SpecificationWrapper(field as Specification);
                else if (type == typeof(Client))
                    wrapper = new ClientWrapper(field as Client);
                else
                    wrapper = new FieldWrapper<FieldT>(field);
                _map[type].Add(wrapper);
                _uuidDic.Add(field.UUID, wrapper);
            }
        }
    }
}