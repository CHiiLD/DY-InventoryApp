using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace R54IN0
{
    public class ItemFieldEditorViewModel : FinderViewModelMediatorColleague, IFieldEditorViewModel
    {
        IFieldWrapper _selectedItem;
        IFieldWrapper _selectedSpecification;

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler DeleteItemCommand { get; set; }

        public CommandHandler AddNewSpecCommand { get; set; }
        public CommandHandler RemoveSpecCommand { get; set; }

        public ObservableCollection<IFieldWrapper> Specifications { get; set; }
        public ObservableCollection<IFieldWrapper> Items { get; set; }

        public IFieldWrapper SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                Specifications.Clear();
                if (_selectedItem != null)
                {
                    ObservableCollection<IFieldWrapper> specColl = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Specification>();
                    IEnumerable<IFieldWrapper> itemSpecColl = specColl.Where(x => ((Specification)x.Field).ItemUUID == _selectedItem.Field.UUID);
                    foreach (var item in itemSpecColl)
                        Specifications.Add(item);
                }
                SelectedSpecification = Specifications.FirstOrDefault();
            }
        }

        public IFieldWrapper SelectedSpecification
        {
            get
            {
                return _selectedSpecification;
            }
            set
            {
                _selectedSpecification = value;
                RemoveSpecCommand.UpdateCanExecute();
            }
        }

        public ItemFieldEditorViewModel() : base(FinderViewModelMediator.GetInstance())
        {
            AddNewItemCommand = new CommandHandler(ExecuteNewItemAddition, CanAddNewItem);
            DeleteItemCommand = new CommandHandler(ExecuteSelectedItemDeletion, CanDeleteSelectedItem);
            AddNewSpecCommand = new CommandHandler(AddNewSpecification, CanAddNewSpecficiation);
            RemoveSpecCommand = new CommandHandler(RemoveSelectedSpecification, CanRemoveSelectedSpecfication);

            Specifications = new ObservableCollection<IFieldWrapper>();
            Items = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Item>();
            SelectedItem = Items.FirstOrDefault();
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanDeleteSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public bool CanAddNewSpecficiation(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedSpecfication(object parameter)
        {
            return SelectedSpecification != null ? Specifications.Count > 1 ? true : false : false;
        }

        public void ExecuteNewItemAddition(object parameter)
        {
            var item = new Item() { Name = "new item", UUID = Guid.NewGuid().ToString() }.Save<Item>();
            var itemPipe = new ItemWrapper(item);
            Items.Add(itemPipe);
            SelectedItem = Items.LastOrDefault();
            // 새로 아이템을 등록할 시 베이스 규격을 등록, 규격 리스트는 최소 하나 이상을 가져야 한다.
            AddNewSpecification(null);
            DeleteItemCommand.UpdateCanExecute();
            UpdateItemPipeCollection(itemPipe, CollectionAction.ADD);
        }

        public void ExecuteSelectedItemDeletion(object parameter)
        {
            UpdateItemPipeCollection(SelectedItem as ItemWrapper, CollectionAction.REMOVE);
            var item = SelectedItem.Field;
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            DeleteItemCommand.UpdateCanExecute();
        }

        public void AddNewSpecification(object parameter)
        {
            var newSpecification = new Specification() { Name = "new specification", ItemUUID = SelectedItem.Field.UUID }.Save<Specification>();
            var newSpecificationPipe = new SpecificationWrapper(newSpecification);
            Specifications.Add(newSpecificationPipe);
            SelectedSpecification = Specifications.LastOrDefault();
            RemoveSpecCommand.UpdateCanExecute();
            FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Specification>().Add(newSpecificationPipe);
        }

        public void RemoveSelectedSpecification(object parameter)
        {
            SelectedSpecification.IsDeleted = true;
            Specifications.Remove(SelectedSpecification);
            SelectedSpecification = Specifications.FirstOrDefault();
            RemoveSpecCommand.UpdateCanExecute();
            FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Specification>().Remove(SelectedSpecification);
        }
    }
}