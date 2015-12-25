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

        FieldWrapperDirector()
        {
            _map = new Dictionary<Type, List<IFieldWrapper>>();
        }

        public static void Distroy()
        {
            if (_thiz != null)
                _thiz._map = null;
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
            }
        }

        public bool Contains<FieldT>(IFieldWrapper item) where FieldT : class, IField
        {
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
            {
                _map[type] = new List<IFieldWrapper>();
                FieldT[] fields = null;
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    fields = db.LoadAll<FieldT>();
                }
#if DEBUG
                foreach (var field in fields)
                    Debug.Assert(field.UUID != null);
#endif

                foreach (var field in fields)
                {
                    if (type == typeof(Item))
                        _map[type].Add(new ItemWrapper(field as Item));
                    else if (type == typeof(Specification))
                        _map[type].Add(new SpecificationWrapper(field as Specification));
                    else if (type == typeof(Client))
                        _map[type].Add(new ClientWrapper(field as Client));
                    else
                        _map[type].Add(new FieldWrapper<FieldT>(field));
                }
            }
            return new ObservableCollection<WrapperT>(_map[type].OfType<WrapperT>());
        }
    }
}