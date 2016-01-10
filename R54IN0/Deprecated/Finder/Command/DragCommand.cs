using System;
using System.Windows.Controls;
using System.Windows.Controls.DragNDrop;
using System.Windows.Input;

namespace R54IN0
{
    public class DragCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExitem = dragParameters.DragItem;
            FinderNode finderNode = treeviewExitem.DataContext as FinderNode;
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