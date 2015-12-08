using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace R54IN0
{
    public interface IFieldEditorViewModel
    {
        CommandHandler AddNewItemCommand { get; set; }
        CommandHandler RemoveItemCommand { get; set; }

        bool CanAddNewItem(object parameter);
        bool CanRemoveSelectedItem(object parameter);
        void AddNewItem(object parameter);
        void RemoveSelectedItem(object parameter);
    }
}