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
    public class FieldCollectionDirector
    {
        static FieldCollectionDirector _thiz;
        List<ObservableCollection<IFieldPipe>> _fieldList;
        List<ObservableCollection<IFieldPipe>> _enableFieldList;

        FieldCollectionDirector()
        {
            _fieldList = new List<ObservableCollection<IFieldPipe>>();
            _enableFieldList = new List<ObservableCollection<IFieldPipe>>();
        }

        public static FieldCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new FieldCollectionDirector();
            return _thiz;
        }

        private void OnEnableCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            IEnumerable enumerable = sender as IEnumerable;
            IFieldPipe iFieldPipe = enumerable.ElementAt(0) as IFieldPipe;
            Type type = iFieldPipe.Field.GetType();
            ObservableCollection<IFieldPipe> result = _fieldList.Where(ob =>
               ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            foreach (var item in e.NewItems)
            {
                IFieldPipe pipe = item as IFieldPipe;
                result.Add(pipe);
            }
        }

        public ObservableCollection<IFieldPipe> LoadEnablePipe<T>() where T : class, IField
        {
            Type type = typeof(T);
            ObservableCollection<IFieldPipe> result = _enableFieldList.Where(ob =>
                ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            if (result != null)
                return result;
            ObservableCollection<IFieldPipe> allObColl = LoadPipe<T>();
            ObservableCollection<IFieldPipe> newColl = new ObservableCollection<IFieldPipe>(allObColl.Where(x => !x.IsDeleted));
            newColl.CollectionChanged += OnEnableCollectionChanged;
            _enableFieldList.Add(newColl);
            return newColl;
        }

        public ObservableCollection<IFieldPipe> LoadPipe<T>() where T : class, IField
        {
            Type type = typeof(T);
            ObservableCollection<IFieldPipe> result = _fieldList.Where(ob =>
                ob.Any(fp => fp.Field.GetType() == type)).SingleOrDefault();
            if (result != null)
                return result;
            ObservableCollection<IFieldPipe> newColl = new ObservableCollection<IFieldPipe>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                IEnumerable<T> pipe = db.LoadAll<T>();
                foreach (T item in pipe)
                {
                    if (type == typeof(Account))
                        newColl.Add(new AccountPipe(item as Account));
                    else if (type == typeof(Item))
                        newColl.Add(new ItemPipe(item as Item));
                    else if (type == typeof(Specification))
                        newColl.Add(new SpecificationPipe(item as Specification));
                    else
                        newColl.Add(new FieldPipe<T>(item));
                }
            }
            _fieldList.Add(newColl);
            return newColl;
        }
    }
}
