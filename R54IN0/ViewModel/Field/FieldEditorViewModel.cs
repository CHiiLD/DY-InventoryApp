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
        public FieldPipe<T> SelectedItem { get; set; }
        public ObservableCollection<FieldPipe<T>> Items { get; set; }

        public CommandHandler AddNewItemCommand { get; set; }
        public CommandHandler RemoveItemCommand { get; set; }

        public FieldEditorViewModel()
        {
            T[] items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<T>();
            }
            IEnumerable<FieldPipe<T>> fieldPipes = items.Where(x => !x.IsDeleted).Select(x => new FieldPipe<T>(x));
            Items = new ObservableCollection<FieldPipe<T>>(fieldPipes);
            SelectedItem = Items.FirstOrDefault();

            AddNewItemCommand = new CommandHandler(AddNewItem, CanAddNewItem);
            RemoveItemCommand = new CommandHandler(RemoveSelectedItem, CanRemoveSelectedItem);
        }

        public bool CanAddNewItem(object parameter)
        {
            return true;
        }

        public bool CanRemoveSelectedItem(object parameter)
        {
            return SelectedItem != null ? true : false;
        }

        public void AddNewItem(object parameter)
        {
            Items.Add(new FieldPipe<T>(new T() { Name = "new" }.Save<T>()));
            SelectedItem = Items.LastOrDefault();
            RemoveItemCommand.UpdateCanExecute();
        }

        public void RemoveSelectedItem(object parameter)
        {
            SelectedItem.IsDeleted = true;
            Items.Remove(SelectedItem);
            SelectedItem = Items.FirstOrDefault();
            RemoveItemCommand.UpdateCanExecute();
        }
    }
}