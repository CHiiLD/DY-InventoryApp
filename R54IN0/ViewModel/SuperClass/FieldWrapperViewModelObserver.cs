using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0
{
    public abstract class FieldWrapperViewModelObserver<FieldT, WrapperT> : CollectionViewModelObserver<WrapperT>
        where FieldT : class, IField
        where WrapperT : class, IFieldWrapper
    {
        protected FieldWrapperDirector fieldWrapperDirector;

        public FieldWrapperViewModelObserver(CollectionViewModelObserverSubject sub) : base(sub)
        {
            fieldWrapperDirector = FieldWrapperDirector.GetInstance();
            var items = fieldWrapperDirector.CreateCollection<FieldT, WrapperT>().Where(x => !x.IsDeleted);
            Items = new ObservableCollection<WrapperT>(items);
        }

        public override void Add(WrapperT item)
        {
            fieldWrapperDirector.Add<FieldT>(item); //순서 중요함
            base.Add(item);
        }

        public override void Remove(WrapperT item)
        {
            item.IsDeleted = true; //Field는 삭제 기능이 없다.
            base.Remove(item);
        }
    }
}