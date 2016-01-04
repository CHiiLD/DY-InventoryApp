using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DragNDrop;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class MultiSelectTreeViewModelView : TreeViewNodeDirector, INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public MultiSelectTreeViewModelView()
        {
            Root = Collection; //new ObservableCollection<TreeViewNode>();
            SelectedNodes = new ObservableCollection<TreeViewNode>();
            DragCommand = new CommandHandler(ExecuteDrag, CanDrag);
            DropCommand = new CommandHandler(ExecuteDrop, CanDrop);
        }

        public ObservableCollection<TreeViewNode> Root { get; set; }

        public ObservableCollection<TreeViewNode> SelectedNodes { get; set; }

        public ICommand DragCommand { get; set; }
        public ICommand DropCommand { get; set; }

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
                if (SelectedNodes.Contains(itemToUnselect as TreeViewNode))
                    SelectedNodes.Remove(itemToUnselect as TreeViewNode);
            }
            foreach (object itemToSelect in e.ItemsToSelect)
            {
                if (!SelectedNodes.Contains(itemToSelect as TreeViewNode))
                    SelectedNodes.Add(itemToSelect as TreeViewNode);
            }
            NotifyPropertyChanged("SelectedNodes");
        }

        public bool CanDrag(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExitem = dragParameters.DragItem;
            TreeViewNode TreeViewNode = treeviewExitem.DataContext as TreeViewNode;
            if (TreeViewNode != null)
                return TreeViewNode.AllowDrag;
            return false;
        }

        public void ExecuteDrag(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExItem = dragParameters.DragItem;
            dragParameters.DraggedObject = treeviewExItem.DataContext;
        }

        public bool CanDrop(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            TreeViewNode des = (treeviewExItem == null) ? null : treeviewExItem.DataContext as TreeViewNode; //넣고자 할 장소(목적지)
            int index = dropParameters.Index;

            if (treeviewExItem == null)
                return true; //drop to root

            foreach (string fmt in dataObject.GetFormats())
            {
                //외부의 드래그를 차단
                if (!(dataObject.GetData(fmt) is DragContent))
                    return false;
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                foreach (object item in dragContent.Items)
                {
                    TreeViewNode src = item as TreeViewNode;
                    //자기노트에 자기자신을 하위로 다시 추가하는 것을 차단
                    if (des == src)
                        return false;
                    //자신의 부모에 이미 자식으로 있음에도 다시 그 부모에 자기 자신을 추가하는 경우를 차단
                    if (des.Root.Any(x => x == src))
                        return false;
                    //부모의 하위 자식에게 부모를 추가하는 경우를 차단
                    if (src.Descendants().Any(x => x == des))
                        return false;
                }
            }
            if (index == -1)
                return des.AllowDrop;
            else
                return des.AllowInsert;
        }

        /// <summary>
        /// 드랍 명령의 실행 기능 메서드
        /// </summary>
        /// <param name="args"></param>
        public void ExecuteDrop(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            int index = dropParameters.Index;
            TreeViewNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as TreeViewNode;
            foreach (string fmt in dataObject.GetFormats())
            {
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                if (dragContent != null)
                {
                    foreach (var selectedNode in dragContent.Items.Reverse())
                    {
                        TreeViewNode node = (TreeViewNode)selectedNode;
                        Remove(node);
                        AddNode(destNode, index, node);
                    }
                }
            }
        }

        private void AddNode(TreeViewNode node, int index, TreeViewNode newNode)
        {
            ObservableCollection<TreeViewNode> children = (node == null) ? Root : node.Root;
            if (index == -1)
            {
                children.Add(newNode);
            }
            else
            {
                if (0 <= index && index < children.Count())
                    children.Insert(index, newNode);
                else
                    children.Add(newNode);
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}