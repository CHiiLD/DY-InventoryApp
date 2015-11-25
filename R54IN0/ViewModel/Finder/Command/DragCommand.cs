using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.DragNDrop;

namespace R54IN0
{
    public class DragCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExitem = dragParameters.DragItem;
            IFinderNode finderNode = treeviewExitem.DataContext as IFinderNode;
            if (finderNode != null)
                return finderNode.AllowDrag;
            return false;
        }

        public void Execute(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExItem = dragParameters.DragItem;
            dragParameters.DraggedObject = treeviewExItem.DataContext;
        }
    }
}
