using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public interface IFieldListEditorViewModel
    {
        ObservableCollection<FieldPipe> Items { get; set; }
        FieldPipe SelectedItem { get; set; }

        void AddNewItem();
        void RemoveSelectedItem();
    }
}