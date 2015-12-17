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
        IFieldPipe _selectedItem;
        IFieldPipe _selectedSpecification;

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler RemoveItemCommand { get; set; }

        public CommandHandler AddNewSpecCommand { get; set; }
        public CommandHandler RemoveSpecCommand { get; set; }

        public ObservableCollection<IFieldPipe> Specifications { get; set; }
        public ObservableCollection<IFieldPipe> Items { get; set; }

        public IFieldPipe SelectedItem
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
                    ObservableCollection<IFieldPipe> specColl = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Specification>();
                    IEnumerable<IFieldPipe> itemSpecColl = specColl.Where(x => ((Specification)x.Field).ItemUUID == _selectedItem.Field.UUID);
                    foreach (var item in itemSpecColl)
                        Specifications.Add(item);
                }
                SelectedSpecification = Specifications.FirstOrDefault();
            }
        }

        public IFieldPipe SelectedSpecification
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
            AddNewItemCommand = new CommandHandler(AddNewItem, CanAddNewItem);
            RemoveItemCommand = new CommandHandler(RemoveSelectedItem, CanRemoveSelectedItem);
            AddNewSpecCommand = new CommandHandler(AddNewSpecification, CanAddNewSpecficiation);
            RemoveSpecCommand = new CommandHandler(RemoveSelectedSpecification, CanRemoveSelectedSpecfication);

            Specifications = new ObservableCollection<IFieldPipe>();
            Items = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Item>();
            SelectedItem = Items.FirstOrDefault();
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedItem(object parameter)
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

        public void AddNewItem(object parameter)
        {
            var item = new Item() { Name = "new item", UUID = Guid.NewGuid().ToString() }.Save<Item>();
            var itemPipe = new ItemPipe(item);
            Items.Add(itemPipe);
            SelectedItem = Items.LastOrDefault();
            // 새로 아이템을 등록할 시 베이스 규격을 등록, 규격 리스트는 최소 하나 이상을 가져야 한다.
            AddNewSpecification(null);
            RemoveItemCommand.UpdateCanExecute();
            UpdateItemPipeCollection(itemPipe, CollectionAction.ADD);
        }

        public void RemoveSelectedItem(object parameter)
        {
            UpdateItemPipeCollection(SelectedItem as ItemPipe, CollectionAction.REMOVE);
            var item = SelectedItem.Field;
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            RemoveItemCommand.UpdateCanExecute();
        }

        public void AddNewSpecification(object parameter)
        {
            var newSpecification = new Specification() { Name = "new specification", ItemUUID = SelectedItem.Field.UUID }.Save<Specification>();
            var newSpecificationPipe = new SpecificationPipe(newSpecification);
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