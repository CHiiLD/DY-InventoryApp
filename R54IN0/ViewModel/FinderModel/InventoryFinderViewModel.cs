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
    public class InventoryFinderViewModel : FinderViewModelMediatorColleague
    {
        DragCommand _dragCommand;
        DropCommand _dropCommand;

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

        public CommandHandler SelectedItemEventCommand { get; set; }

        public InventoryFinderViewModel() : base(FinderViewModelMediator.GetInstance())
        {
            Nodes = new ObservableCollection<FinderNode>();
            SelectedNodes = new ObservableCollection<FinderNode>();
            _dragCommand = new DragCommand();
            _dropCommand = new DropCommand(this);

            RemoveDirectoryCommand = new CommandHandler(RemoveSelectedDirectories, CanRemoveSelectedDirectoies);
            AddNewDirectoryCommand = new CommandHandler(AddNewDirectories, CanAddNewDirectory);
        }

        public InventoryFinderViewModel(TreeViewEx treeView) : this()
        {
            Nodes = FinderNodeCollectionDirector.GetInstance().Collection;
            treeView.OnSelecting += OnSelectNodes;
        }

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
            base.ShowSelectedFinderNodes(this);
            ((CommandHandler)RemoveDirectoryCommand).UpdateCanExecute();
        }

        public void AddNewDirectoryInSelectedDirectory()
        {
            if (SelectedNodes.Count == 0)
            {
                Nodes.Add(new FinderNode(NodeType.DIRECTORY) { Name = "New Directory" });
                return;
            }
            IEnumerable<FinderNode> diNode = SelectedNodes.Where(x => x.Type == NodeType.DIRECTORY);
            if (diNode.Count() != 0)
                diNode.First().Nodes.Add(new FinderNode(NodeType.DIRECTORY) { Name = "New Directory" });
            else
                Nodes.Add(new FinderNode(NodeType.DIRECTORY) { Name = "New Directory" });
        }

        internal void RemoveNodeInRoot(FinderNode node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
                return;
            }
            FinderNode result = Nodes.SelectMany(x => x.Descendants().Where(y => y.Nodes.Contains(node))).SingleOrDefault();
            if (result != null)
                result.Nodes.Remove(node);
        }

        public void DeleteSelectedDirectories()
        {
            ObservableCollection<FinderNode> copy = new ObservableCollection<FinderNode>(SelectedNodes);
            //tree구조의 노드를 일차Collection으로 모운 뒤, itemnode를 찾아서 ROOT노드에 중복 없이 추가
            IEnumerable<FinderNode> itemNodes = copy.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM));
            foreach (FinderNode node in itemNodes)
            {
                if (Nodes.All(n => n.UUID != node.UUID))
                    Nodes.Add(new FinderNode(node));
            }
            //부모노드에서 삭제
            foreach (FinderNode node in copy)
            {
                if (!(node.Type == NodeType.ITEM && Nodes.Contains(node)))
                    RemoveNodeInRoot(node);
            }
            SelectedNodes.Clear();
        }

        public void AddNewItemInNodes(string itemUUID)
        {
            Nodes.Add(new FinderNode(NodeType.ITEM) { ItemUUID = itemUUID });
        }

        public void RemoveItemInNodes(string itemUUID)
        {
            IEnumerable<FinderNode> itemNodes = Nodes.SelectMany(n => n.Descendants().Where(x => x.Type == NodeType.ITEM));
            itemNodes = itemNodes.Where(t => t.ItemUUID == itemUUID);
            var node = itemNodes.SingleOrDefault();
            if (node != null)
                RemoveNodeInRoot(node);
        }
    }
}