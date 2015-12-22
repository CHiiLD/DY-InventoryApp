using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;

namespace R54IN0
{
    public class FinderViewModel : INotifyPropertyChanged
    {
        public FinderViewModel(TreeViewEx treeView)
        {
            SelectedNodes = new ObservableCollection<FinderNode>();
            if (treeView != null)
                treeView.OnSelecting += OnNodeSelected;
        }

        public FinderViewModel(TreeViewEx treeView, ObservableCollection<FinderNode> root) : this(treeView)
        {
            Nodes = root;
        }

        public virtual ObservableCollection<FinderNode> Nodes { get; private set; }
        public virtual ObservableCollection<FinderNode> SelectedNodes { get; set; }

        public EventHandler SelectItemsChanged { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// TreeViewEx의 OnSelecting 이벤트와 연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnNodeSelected(object sender, SelectionChangedCancelEventArgs e)
        {
            e.Cancel = true;

            foreach (object itemToUnselect in e.ItemsToUnSelect)
            {
                if (SelectedNodes.Contains(itemToUnselect as FinderNode))
                    SelectedNodes.Remove(itemToUnselect as FinderNode);
            }

            foreach (object itemToSelect in e.ItemsToSelect)
            {
                if (!SelectedNodes.Contains(itemToSelect as FinderNode))
                    SelectedNodes.Add(itemToSelect as FinderNode);
            }

            if (SelectItemsChanged != null)
                SelectItemsChanged(this, EventArgs.Empty);

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedNodes"));
        }
    }
}