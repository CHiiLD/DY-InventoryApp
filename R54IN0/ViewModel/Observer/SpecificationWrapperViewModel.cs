using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class SpecificationWrapperViewModel : FieldWrapperViewModel<Specification, SpecificationWrapper>
    {
        ItemWrapperViewModel _itemViewModel;

        public SpecificationWrapperViewModel(CollectionViewModelObserverSubject sub, ItemWrapperViewModel itemViewModel) : base(sub)
        {
            _itemViewModel = itemViewModel;
        }

        public override bool CanDeleteSelectedItem(object parameter)
        {
            //최소 하나 이하를 삭제할 수 없다.
            return base.CanDeleteSelectedItem(parameter) && Items.Count > 1;
        }

        public override bool CanAddNewItem(object parameter)
        {
            return _itemViewModel.SelectedItem != null ? true : false;
        }

        public override void ExecuteNewItemAddition(object parameter)
        {
            base.ExecuteNewItemAddition(parameter);
            SelectedItem.Field.ItemUUID = _itemViewModel.SelectedItem.UUID;
            SelectedItem.Field.Save<Specification>();
            _itemViewModel.SelectedSpecification = SelectedItem;
        }
    }
}