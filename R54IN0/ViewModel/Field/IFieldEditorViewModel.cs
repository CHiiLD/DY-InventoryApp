using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IFieldEditorViewModel
    {
        void Save();
        void AddNewItem();
        void RemoveSelectedItem();    
    }
}