using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace R54IN0
{
    public class NewDirectoryAddCommand : ICommand
    {
        InventoryFinderViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public NewDirectoryAddCommand(InventoryFinderViewModel viewmodel)
        {
            _viewModel = viewmodel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel.SelectedNodes.Count != 0;
        }

        public void Execute(object parameter)
        {
            if (_viewModel.SelectedNodes.Count == 0)
                return;
            foreach (var node in _viewModel.SelectedNodes)
            {
                if (node.GetType() == typeof(DirectoryNode))
                {
                    _viewModel.SelectedNodes.Add(new DirectoryNode("New Directory"));
                    break;
                }
            }
        }
    }
}
