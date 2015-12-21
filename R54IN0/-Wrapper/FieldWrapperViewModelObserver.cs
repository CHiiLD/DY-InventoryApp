using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0
{
    public class FieldWrapperViewModelObserver<FieldT, WrapperT> : ViewModelObserver<WrapperT>
        where FieldT : class, IField
        where WrapperT : class, IFieldWrapper
    {
        protected FieldWrapperDirector fieldWrapperDirector;

        public FieldWrapperViewModelObserver(ViewModelObserverSubject sub) : base(sub)
        {
            fieldWrapperDirector = FieldWrapperDirector.GetInstance();
            var items = fieldWrapperDirector.CreateCollection<FieldT, WrapperT>().Where(x => !x.IsDeleted);
            Items = new ObservableCollection<WrapperT>(items);
        }

        public override void Add(WrapperT item)
        {
            base.Add(item);
            fieldWrapperDirector.Add<FieldT>(item);
        }

        public override void Remove(WrapperT item)
        {
            item.IsDeleted = true; //Field는 삭제 기능이 없다.
            base.Remove(item);
        }
    }
}