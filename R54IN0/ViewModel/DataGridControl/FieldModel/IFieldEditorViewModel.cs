using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public interface IFieldEditorViewModel
    {
        IFieldWrapper SelectedItem { get; set; }
        ObservableCollection<IFieldWrapper> Items { get; set; }

        CommandHandler AddNewItemCommand { get; set; }
        CommandHandler DeleteItemCommand { get; set; }

        bool CanAddNewItem(object parameter);
        bool CanDeleteSelectedItem(object parameter);
        void ExecuteNewItemAddition(object parameter);
        void ExecuteSelectedItemDeletion(object parameter);
    }
}