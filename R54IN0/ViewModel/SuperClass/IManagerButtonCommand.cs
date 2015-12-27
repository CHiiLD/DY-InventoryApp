using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public interface IButtonCommands
    {
        CommandHandler AddCommand { get; set; }
        CommandHandler RemoveCommand { get; set; }
        CommandHandler ModifyCommand { get; set; }

        bool CanAddNewItem(object parameter);
        bool CanModifySelectedItem(object parameter);
        bool CanRemoveSelectedItem(object parameter);

        void ExecuteAddCommand(object parameter);
        void ExecuteModifyCommand(object parameter);
        void ExecuteRemoveCommand(object parameter);
    }
}