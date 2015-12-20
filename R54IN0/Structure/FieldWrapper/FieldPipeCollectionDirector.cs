using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace R54IN0
{
    public class FieldPipeCollectionDirector
    {
        static FieldPipeCollectionDirector _thiz;
        List<ObservableCollection<IFieldWrapper>> _fieldList;
        List<ObservableCollection<IFieldWrapper>> _enableFieldList;

        FieldPipeCollectionDirector()
        {
            _fieldList = new List<ObservableCollection<IFieldWrapper>>();
            _enableFieldList = new List<ObservableCollection<IFieldWrapper>>();
        }

        public static FieldPipeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FieldPipeCollectionDirector();
            return _thiz;
        }
        public ObservableCollection<IFieldWrapper> LoadEnablePipe<T>() where T : class, IField
        {
            Type type = typeof(T);
            ObservableCollection<IFieldWrapper> result = _enableFieldList.Where(ob =>
                ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            if (result != null)
                return result;
            ObservableCollection<IFieldWrapper> allObColl = LoadPipe<T>();
            ObservableCollection<IFieldWrapper> newColl = new ObservableCollection<IFieldWrapper>(allObColl.Where(x => !x.IsDeleted));
            newColl.CollectionChanged += OnEnableCollectionChanged;
            _enableFieldList.Add(newColl);
            return newColl;
        }

        public ObservableCollection<IFieldWrapper> LoadPipe<T>() where T : class, IField
        {
            Type type = typeof(T);
            ObservableCollection<IFieldWrapper> result = _fieldList.Where(ob =>
                ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            if (result != null)
                return result;
            ObservableCollection<IFieldWrapper> newColl = new ObservableCollection<IFieldWrapper>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                IEnumerable<T> pipe = db.LoadAll<T>();
                foreach (T item in pipe)
                {
                    if (type == typeof(Account))
                        newColl.Add(new AccountWrapper(item as Account));
                    else if (type == typeof(Item))
                        newColl.Add(new ItemWrapper(item as Item));
                    else if (type == typeof(Specification))
                        newColl.Add(new SpecificationWrapper(item as Specification));
                    else
                        newColl.Add(new FieldWrapper<T>(item));
                }
            }
            _fieldList.Add(newColl);
            return newColl;
        }

        void OnEnableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            IEnumerable enumerable = sender as IEnumerable;
            IFieldWrapper iFieldPipe = enumerable.ElementAt(0) as IFieldWrapper;
            Type type = iFieldPipe.Field.GetType();
            ObservableCollection<IFieldWrapper> result = _fieldList.Where(ob =>
               ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            foreach (var item in e.NewItems)
            {
                IFieldWrapper pipe = item as IFieldWrapper;
                result.Add(pipe);
            }
        }
    }
}
