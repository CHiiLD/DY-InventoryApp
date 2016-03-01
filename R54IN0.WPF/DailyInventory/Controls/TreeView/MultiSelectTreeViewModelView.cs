using GalaSoft.MvvmLight.Command;
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
    /// <summary>
    /// MultiSelectTreeView ViewModel 클래스
    /// </summary>
    public partial class MultiSelectTreeViewModelView : INotifyPropertyChanged, ICollectionViewModelObserver
    {
        private TreeViewNodeDirector _director;
        private ObservableCollection<TreeViewNode> _selectedNodes;
        private Visibility _newFolderAddVisibility;
        private Visibility _newProductAddVisibility;
        private Visibility _contextMenuVisibility;

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
            _director = TreeViewNodeDirector.GetInstance();
            Root = _director.Collection;
            SelectedNodes = new ObservableCollection<TreeViewNode>();

            NewFolderAddMenuVisibility = Visibility.Visible;
            NewProductAddMenuVisibility = Visibility.Visible;
            ContextMenuVisibility = Visibility.Visible;

            //Drag & Drop
            DragCommand = new RelayCommand<DragParameters>(ExecuteDrag, CanDrag);
            DropCommand = new RelayCommand<DropParameters>(ExecuteDrop, CanDrop);

            //mouse click
            NodesSelectedEventCommand = new RelayCommand<SelectionChangedCancelEventArgs>(ExecuteNodesSelectedEventCommand);
            MouseRightButtonDownEventCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteMouseRightButtonDownEventCommand);

            //Context Menu
            SelectedNodeRenameCommand = new RelayCommand(ExecuteSelectedNodeRenameCommand, IsOnlyOne);
            NewFolderNodeAddCommand = new RelayCommand(ExecuteNewFolderNodeAddCommand, CanAddFolderNode);
            NewProductNodeAddCommand = new RelayCommand(ExecuteNewProductNodeAddCommand, CanAddProductNode);
            NewInventoryNodeAddCommand = new RelayCommand(ExecuteNewInventoryNodeAddCommand, CanAddInventoryNode);
            SearchAsIOStockRecordCommand = new RelayCommand(ExecuteSearchAsIOStockRecordCommand, CanExecuteSearchAsIOStockRecordCommand);
            SearchAsInventoryRecordCommand = new RelayCommand(ExecuteSearchAsInventoryRecordCommand, CanExecuteSearchAsInventoryRecordCommand);
            StockModifyCommand = new RelayCommand(ExecuteStockModifyCommand, CanExecuteStockModifyCommand);
            SelectedNodeDeletionCommand = new RelayCommand(ExecuteSelectedNodeDeletionCommand, CanDeleteNode);
            InventoryModifyCommand = new RelayCommand(ExecuteInventoryModifyCommand, CanExecuteInventoryModifyCommand); //inventory 수정하기

            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        ~MultiSelectTreeViewModelView()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        public ObservableCollection<TreeViewNode> Root { get; private set; }

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

        /// <summary>
        /// ContextMenu의 MenuItem(새로운 폴더 추가)의 가시 여부
        /// </summary>
        public Visibility NewFolderAddMenuVisibility
        {
            get
            {
                return _newFolderAddVisibility;
            }
            set
            {
                _newFolderAddVisibility = value;
                NotifyPropertyChanged("NewFolderAddVisibility");
            }
        }

        /// <summary>
        /// ContextMenu의 MenuItem(새로운 제품 추가)의 가시 여부
        /// </summary>
        public Visibility NewProductAddMenuVisibility
        {
            get
            {
                return _newProductAddVisibility;
            }
            set
            {
                _newProductAddVisibility = value;
                NotifyPropertyChanged("NewProductAddVisibility");
            }
        }

        /// <summary>
        /// ContextMenu의 가시 여부
        /// </summary>
        public Visibility ContextMenuVisibility
        {
            get
            {
                return _contextMenuVisibility;
            }
            set
            {
                _contextMenuVisibility = value;
                NotifyPropertyChanged("ContextMenuVisibility");
            }
        }

        public RelayCommand<DragParameters> DragCommand { get; set; }

        public RelayCommand<DropParameters> DropCommand { get; set; }

        /// <summary>
        /// 드래그 허용 여부
        /// </summary>
        /// <param name="dragParameters"></param>
        /// <returns></returns>
        public bool CanDrag(DragParameters dragParameters)
        {
            TreeViewExItem treeviewExitem = dragParameters.DragItem;
            TreeViewNode treeViewNode = treeviewExitem.DataContext as TreeViewNode;
            if (treeViewNode != null)
                return treeViewNode.AllowDrag;
            return false;
        }

        /// <summary>
        /// 드래그 기능을 실행한다.
        /// </summary>
        /// <param name="dragParameters"></param>
        private void ExecuteDrag(DragParameters dragParameters)
        {
            TreeViewExItem treeviewExItem = dragParameters.DragItem;
            dragParameters.DraggedObject = treeviewExItem.DataContext;
        }

        /// <summary>
        /// 드랍 허용
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool CanDrop(DropParameters dropParameters)
        {
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            if (dataObject.GetFormats().Any(x => x != "System.Windows.Controls.DragNDrop.DragContent")) //외부의 드래그를 차단
                return false;
            var fmt = dataObject.GetFormats().SingleOrDefault(); //fmt 데이터는 항상 하나가 있기를 기대함
            if (fmt == null)
                return false;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            if (treeviewExItem == null)
                return true; //drop to root

            TreeViewNode destnode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as TreeViewNode; //넣고자 할 장소(목적지)
            int index = dropParameters.Index;
            DragContent dragContent = dataObject.GetData(fmt) as DragContent;
            IEnumerable<TreeViewNode> nodes = dragContent.Items.OfType<TreeViewNode>();

            foreach (TreeViewNode selectedNode in nodes)
            {
                //자기노트에 자기자신을 하위로 다시 추가하는 것을 차단
                if (destnode == selectedNode)
                    return false;
                //자신의 부모에 이미 자식으로 있음에도 다시 그 부모에 자기 자신을 추가하는 경우를 차단
                //if (destnode.Root.Any(x => x == selectedNode))
                //    return false;
                //부모의 하위 자식에게 부모를 추가하는 경우를 차단
                if (selectedNode.Descendants().Any(x => x == destnode))
                    return false;
                //부모와 자식이 같이 선택된 경우
                if (nodes.Any(ns => ns.Descendants().Any(n => n != selectedNode && n.Root.Any(c => c == selectedNode))))
                    return false;
            }
            if (index == -1)
                return destnode.AllowDrop;
            else
                return destnode.AllowInsert;
        }

        /// <summary>
        /// 드랍 기능을 실행한다.
        /// </summary>
        /// <param name="args"></param>
        private void ExecuteDrop(DropParameters dropParameters)
        {
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
                _director.Remove(src);
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

        /// <summary>
        /// 선택된 노드들 중에서 노드타입에 해당하는 노드들을 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TreeViewNode> SearchNodeInSelectedNodes(NodeType type)
        {
            var children = SelectedNodes.SelectMany(x => x.Descendants());
            return children.Where(x => type.HasFlag(x.Type)).ToList();
        }

        /// <summary>
        /// 전체 노드들 중에서 노드타입에 해당하는 노드를 반환합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<TreeViewNode> SearchNodesInRoot(NodeType type)
        {
            var children = Root.SelectMany(x => x.Descendants());
            return children.Where(x => type.HasFlag(x.Type)).ToList();
        }

        public TreeViewNode SearchNodeInRoot(string id)
        {
            List<TreeViewNode> nodes = SearchNodesInRoot(NodeType.PRODUCT | NodeType.INVENTORY);
            if (nodes.Any(x => x.ObservableObjectID == id))
                return nodes.Where(x => x.ObservableObjectID == id).Single();
            else
                return null;
        }

        /// <summary>
        /// SelectedNodes 프로퍼티에 새로운 노드를 추가합니다.
        /// </summary>
        /// <param name="node"></param>
        public void AddSelectedNodes(TreeViewNode node)
        {
            if (!_director.Contains(node))
                throw new ArgumentException();
            ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void UpdateNewItem(object item)
        {
            if (item is Observable<Product>)
            {
                Observable<Product> prod = item as Observable<Product>;
                TreeViewNode node = SearchNodeInRoot(prod.ID);
                if (node == null)
                    Root.Add(new TreeViewNode(prod));
            }
            else if (item is ObservableInventory)
            {
                ObservableInventory inv = item as ObservableInventory;
                TreeViewNode node = SearchNodeInRoot(inv.ProductID);
                if (node != null && node.Root.All(x => x.ObservableObjectID != inv.ID))
                    node.Root.Add(new TreeViewNode(inv));
            }
        }

        public void UpdateDelItem(object item)
        {
            if (item is Observable<Product>)
            {
                Observable<Product> prod = item as Observable<Product>;
                TreeViewNode node = SearchNodeInRoot(prod.ID);
                if (node != null)
                    _director.Remove(node);
            }
            else if (item is ObservableInventory)
            {
                ObservableInventory inv = item as ObservableInventory;
                TreeViewNode node = SearchNodeInRoot(inv.ID);
                if (node != null)
                    _director.Remove(node);
            }
        }
    }
}