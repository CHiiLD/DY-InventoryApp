namespace R54IN0
{
    public abstract class ItemSourceViewModel<T> : CollectionViewModelObserver<T>, IButtonCommands, ICollectionViewModel<T> where T : class
    {
        private T _selectedItem;

        public ItemSourceViewModel(CollectionViewModelObserverSubject subject) : base(subject)
        {
            AddCommand = new RelayCommand<object>(ExecuteAddCommand, CanAddNewItem);
            ModifyCommand = new RelayCommand<object>(ExecuteModifyCommand, CanModifySelectedItem);
            RemoveCommand = new RelayCommand<object>(ExecuteRemoveCommand, CanRemoveSelectedItem);
        }

        public RelayCommand<object> AddCommand { get; set; }
        public RelayCommand<object> RemoveCommand { get; set; }
        public RelayCommand<object> ModifyCommand { get; set; }

        public virtual T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                ModifyCommand.RaiseCanExecuteChanged();
                RemoveCommand.RaiseCanExecuteChanged();
            }
        }

        public virtual bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public virtual bool CanModifySelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public virtual bool CanRemoveSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public abstract void ExecuteAddCommand(object parameter);

        public abstract void ExecuteModifyCommand(object parameter);

        public abstract void ExecuteRemoveCommand(object parameter);
    }
}