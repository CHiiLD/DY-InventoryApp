using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace R54IN0
{
    public class FieldEditorViewModel<T> : IFieldEditorViewModel where T : class, IField, new()
    {
        public IFieldWrapper SelectedItem { get; set; }
        public ObservableCollection<IFieldWrapper> Items { get; set; }

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler DeleteItemCommand { get; set; }

        public FieldEditorViewModel()
        {
            Items = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<T>();
            SelectedItem = Items.FirstOrDefault();

            AddNewItemCommand = new CommandHandler(ExecuteNewItemAddition, CanAddNewItem);
            DeleteItemCommand = new CommandHandler(ExecuteSelectedItemDeletion, CanDeleteSelectedItem);
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanDeleteSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public void ExecuteNewItemAddition(object parameter)
        {
            Items.Add(new FieldWrapper<T>(new T() { Name = "new" }.Save<T>()));
            SelectedItem = Items.LastOrDefault();
            DeleteItemCommand.UpdateCanExecute();
        }

        public void ExecuteSelectedItemDeletion(object parameter)
        {
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            DeleteItemCommand.UpdateCanExecute();
        }
    }
}