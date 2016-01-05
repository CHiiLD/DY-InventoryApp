using System;
using System.Collections.Generic;
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
        ObservableCollection<TreeViewNode> _selectedNodes;

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
            Root = Collection;
            SelectedNodes = new ObservableCollection<TreeViewNode>();
            DragCommand = new CommandHandler(ExecuteDrag, CanDrag);
            DropCommand = new CommandHandler(ExecuteDrop, CanDrop);
        }

        public ObservableCollection<TreeViewNode> Root { get; set; }

        public ObservableCollection<TreeViewNode> SelectedNodes
        {
            get
            {
                return _selectedNodes;
            }
            set
            {
                _selectedNodes = value;
                NotifyPropertyChanged("SelectedNodes");
            }
        }

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

        /// <summary>
        /// 드래그 허용 여부
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CanDrag(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExitem = dragParameters.DragItem;
            TreeViewNode TreeViewNode = treeviewExitem.DataContext as TreeViewNode;
            if (TreeViewNode != null)
                return TreeViewNode.AllowDrag;
            return false;
        }

        /// <summary>
        /// 드래그 실행 
        /// </summary>
        /// <param name="args"></param>
        public void ExecuteDrag(object args)
        {
            DragParameters dragParameters = args as DragParameters;
            TreeViewExItem treeviewExItem = dragParameters.DragItem;
            dragParameters.DraggedObject = treeviewExItem.DataContext;
        }


        /// <summary>
        /// 드랍 허용 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CanDrop(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            if (dataObject.GetFormats().Any(x => x != "System.Windows.Controls.DragNDrop.DragContent")) //외부의 드래그를 차단
                return false;
            var fmt = dataObject.GetFormats().SingleOrDefault(); //fmt 데이터는 항상 하나가 있기를 기대함
            if (fmt == null)
                return false;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            if (treeviewExItem == null)
                return true; //drop to root

            TreeViewNode des = (treeviewExItem == null) ? null : treeviewExItem.DataContext as TreeViewNode; //넣고자 할 장소(목적지)
            int index = dropParameters.Index;
            DragContent dragContent = dataObject.GetData(fmt) as DragContent;
            IEnumerable<TreeViewNode> nodes = dragContent.Items.OfType<TreeViewNode>();

            foreach (TreeViewNode src in nodes)
            {
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
            TreeViewNode des = (treeviewExItem == null) ? null : treeviewExItem.DataContext as TreeViewNode;

            var fmt = dataObject.GetFormats().SingleOrDefault(); //fmt 데이터는 항상 하나가 있기를 기대함
            if (fmt == null)
                return;
            DragContent dragContent = dataObject.GetData(fmt) as DragContent;
            IEnumerable<TreeViewNode> nodes = dragContent.Items.OfType<TreeViewNode>();

            foreach (TreeViewNode src in nodes.Reverse())
            {
                Remove(src);
                AddNode(des, index, src);
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