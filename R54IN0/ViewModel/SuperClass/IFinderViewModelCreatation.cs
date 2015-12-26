using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace R54IN0
{
    public interface IFinderViewModelCreatation : IFinderViewModelOnSelectingCallback
    {
        FinderViewModel FinderViewModel { get; set; }
        FinderViewModel CreateFinderViewModel(TreeViewEx treeView);
    }
}
