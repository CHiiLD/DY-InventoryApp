using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls.DragNDrop;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using Lex.Db;

namespace R54IN0
{
    public class FinderViewModel// : FinderViewModelMediatorColleague
    {
        FinderDirector _finderDirector;
        DragCommand _dragCommand;
        DropCommand _dropCommand;

        public FinderViewModel() //: base(FinderViewModelMediator.GetInstance())
        {
            if (Nodes == null)
                Nodes = new ObservableCollection<FinderNode>();
            SelectedNodes = new ObservableCollection<FinderNode>();
            _dragCommand = new DragCommand();
            _dropCommand = new DropCommand(this);

            RemoveDirectoryCommand = new CommandHandler(RemoveSelectedDirectories, CanRemoveSelectedDirectoies);
            AddNewDirectoryCommand = new CommandHandler(AddNewDirectories, CanAddNewDirectory);
        }

        public FinderViewModel(TreeViewEx treeView) : this()
        {
            _finderDirector = FinderDirector.GetInstance();
            Nodes = _finderDirector.Collection;
            if (treeView != null)
                treeView.OnSelecting += OnSelectNodes;
        }

        public ObservableCollection<FinderNode> Nodes { get; set; }

        public ObservableCollection<FinderNode> SelectedNodes { get; set; }

        public ICommand DragCommand
        {
            get
            {
                return _dragCommand;
            }
        }

        public ICommand DropCommand
        {
            get
            {
                return _dropCommand;
            }
        }

        public ICommand AddNewDirectoryCommand { get; set; }
        public ICommand RemoveDirectoryCommand { get; set; }

        public EventHandler SelectItemsChanged { get; set; }

        public bool CanAddNewDirectory(object pamateter)
        {
            return true;
        }

        public void AddNewDirectories(object parameter)
        {
            AddNewDirectoryInSelectedDirectory();
        }

        public bool CanRemoveSelectedDirectoies(object parameter)
        {
            if (SelectedNodes.Count == 0)
                return false;
            if (SelectedNodes.Any(x => x.Type == NodeType.ITEM))
                return false;
            return true;
        }

        public void RemoveSelectedDirectories(object parameter)
        {
            DeleteSelectedDirectories();
        }

        /// <summary>
        /// TreeViewEx의 OnSelect 이벤트와 연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSelectNodes(object sender, SelectionChangedCancelEventArgs e)
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
            //base.ShowSelectedFinderNodes(this);
            ((CommandHandler)RemoveDirectoryCommand).UpdateCanExecute();

            if (SelectItemsChanged != null)
                SelectItemsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// 새로운 아이템을 디렉터리에 추가한다.
        /// </summary>
        public void AddNewDirectoryInSelectedDirectory()
        {
            IEnumerable<FinderNode> directoryNodeInSelection = SelectedNodes.Where(x => x.Type == NodeType.DIRECTORY);
            var newNode = new FinderNode(NodeType.DIRECTORY) { Name = "New Directory" };
            if (directoryNodeInSelection.Count() != 0)
                _finderDirector.Add(directoryNodeInSelection.First(), newNode);
            else
                _finderDirector.Add(newNode);
        }

        public void DeleteSelectedDirectories()
        {
            ObservableCollection<FinderNode> copy = new ObservableCollection<FinderNode>(SelectedNodes);
            foreach (var node in copy)
                _finderDirector.Remove(node);
            SelectedNodes.Clear();
        }

        public void AddNewItemInNodes(string itemUUID)
        {
            Nodes.Add(new FinderNode(NodeType.ITEM) { ItemUUID = itemUUID });
        }












        //호흡기들
        public void RemoveItemInNodes(string itemUUID)
        {
            IEnumerable<FinderNode> itemNodes = Nodes.SelectMany(n => n.Descendants().Where(x => x.Type == NodeType.ITEM));
            itemNodes = itemNodes.Where(t => t.ItemUUID == itemUUID);
            var node = itemNodes.SingleOrDefault();
            if (node != null)
                RemoveNodeInRoot(node);
        }

        internal void RemoveNodeInRoot(FinderNode node)
        {
            _finderDirector.Remove(node);
        }
    }
}