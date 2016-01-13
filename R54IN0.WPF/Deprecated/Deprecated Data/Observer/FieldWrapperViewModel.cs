using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace R54IN0
{
    public class FieldWrapperViewModel<FieldT, WrapperT> : FieldWrapperViewModelObserver<FieldT, WrapperT>,
        INotifyPropertyChanged, ICollectionViewModel<WrapperT>
        where FieldT : class, IField, new()
        where WrapperT : class, IObservableField, new()
    {
        private ObservableCollection<WrapperT> _items;
        private WrapperT _selectedItem;

        public FieldWrapperViewModel(CollectionViewModelObserverSubject sub) : base(sub)
        {
            AddNewItemCommand = new RelayCommand<object>(ExecuteNewItemAddition, CanAddNewItem);
            DeleteItemCommand = new RelayCommand<object>(ExecuteSelectedItemDeletion, CanDeleteSelectedItem);
        }

        public RelayCommand<object> AddNewItemCommand
        {
            get; set;
        }

        public RelayCommand<object> DeleteItemCommand
        {
            get; set;
        }

        public override ObservableCollection<WrapperT> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged("Items");
            }
        }

        public virtual WrapperT SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
                DeleteItemCommand.RaiseCanExecuteChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public virtual bool CanDeleteSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public virtual void ExecuteNewItemAddition(object parameter)
        {
            FieldT field = new FieldT() { Name = "새로운 기록" };
            WrapperT wrapper = new WrapperT();
            wrapper.Field = field;
            Add(wrapper);

            SelectedItem = Items.LastOrDefault();
            DeleteItemCommand.RaiseCanExecuteChanged();
        }

        public virtual void ExecuteSelectedItemDeletion(object parameter)
        {
            Remove(SelectedItem);
            SelectedItem = null;
            DeleteItemCommand.RaiseCanExecuteChanged();
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}