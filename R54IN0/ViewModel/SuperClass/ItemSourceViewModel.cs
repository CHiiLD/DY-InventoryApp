using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public abstract class ItemSourceViewModel<T> : CollectionViewModelObserver<T>, ICollectionViewModel<T> where T : class
    {
        T _selectedItem;

        public ItemSourceViewModel(CollectionViewModelObserverSubject subject) : base(subject)
        {
            AddCommand = new CommandHandler(ExecuteAddCommand, CanAddNewItem);
            ModifyCommand = new CommandHandler(ExecuteModifyCommand, CanModifySelectedItem);
            RemoveCommand = new CommandHandler(ExecuteRemoveCommand, CanRemoveSelectedItem);
        }

        public CommandHandler AddCommand { get; set; }
        public CommandHandler RemoveCommand { get; set; }
        public CommandHandler ModifyCommand { get; set; }

        public virtual T SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                ModifyCommand.UpdateCanExecute();
                RemoveCommand.UpdateCanExecute();
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