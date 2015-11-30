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
    public class InventoryFinderViewModel
    {
        DragCommand _dragCommand;
        DropCommand _dropCommand;

        public ObservableCollection<DirectoryNode> Nodes { get; set; }
        public ObservableCollection<DirectoryNode> SelectedNodes { get; set; }
        private const string JSON_TREE_KEY = "1233kfasd-flkdks-232a";

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

        public Action<InventoryFinderViewModel> SelectingAction { get; set; }

        public InventoryFinderViewModel()
        {
            Nodes = new ObservableCollection<DirectoryNode>();
            SelectedNodes = new ObservableCollection<DirectoryNode>();
            _dragCommand = new DragCommand();
            _dropCommand = new DropCommand(this);
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
                if (SelectedNodes.Contains(itemToUnselect as DirectoryNode))
                    SelectedNodes.Remove(itemToUnselect as DirectoryNode);
            }

            foreach (object itemToSelect in e.ItemsToSelect)
            {
                if (!SelectedNodes.Contains(itemToSelect as DirectoryNode))
                    SelectedNodes.Add(itemToSelect as DirectoryNode);
            }

            if (SelectingAction != null)
                SelectingAction(this);
        }

        public void AddNewDirectoryInSelectedDirectory()
        {
            if (SelectedNodes.Count == 0)
            {
                Nodes.Add(new DirectoryNode("New Directory"));
                return;
            }
            IEnumerable<DirectoryNode> diNode = SelectedNodes.Where(x => x.GetType() == typeof(DirectoryNode));
            if (diNode.Count() != 0)
                diNode.First().Nodes.Add(new DirectoryNode("New Directory"));
            else
                Nodes.Add(new DirectoryNode("New Directory"));
        }

        internal void RemoveNodeInRoot(DirectoryNode node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
                return;
            }
            DirectoryNode result = Nodes.SelectMany(x => x.Descendants().Where(y => y.Nodes.Contains(node))).SingleOrDefault();
            if (result != null)
                result.Nodes.Remove(node);
        }

        public void DeleteSelectedDirectories()
        {
            ObservableCollection<DirectoryNode> selecNodeCpy = new ObservableCollection<DirectoryNode>(SelectedNodes);
            //tree구조의 노드를 일차Collection으로 모운 뒤, itemnode를 찾아서 ROOT노드에 중복 없이 추가
            IEnumerable<ItemNode> itemNodes = selecNodeCpy.SelectMany(x => x.Descendants().OfType<ItemNode>());
            foreach (DirectoryNode node in itemNodes)
            {
                if (Nodes.All(n => n.UUID != node.UUID))
                    Nodes.Add(new ItemNode(node as ItemNode));
            }
            //부모노드에서 삭제
            foreach (DirectoryNode node in selecNodeCpy)
            {
                if (!(node.GetType() == typeof(ItemNode) && Nodes.Contains(node)))
                    RemoveNodeInRoot(node);
            }
            SelectedNodes.Clear();
        }

        public void AddNewItemInNodes(string itemUUID)
        {
            Nodes.Add(new ItemNode(itemUUID));
        }

        public void RemoveItemInNodes(string itemUUID)
        {
#if !DEBUG
            try
            {
#endif
            IEnumerable<ItemNode> itemNodes = Nodes.SelectMany(n => n.Descendants().OfType<ItemNode>());
            itemNodes = itemNodes.Where(t => t.ItemUUID == itemUUID);
            var node = itemNodes.SingleOrDefault();
            if (node != null)
                RemoveNodeInRoot(node);
#if !DEBUG
        }
            catch(Exception e)
            {

            }
#endif
        }

        public void SaveTree()
        {
            string json = JsonConvert.SerializeObject(this.Nodes);
            using (var db = DatabaseDirector.GetDbInstance())
            {
                db.Save(new SimpleStringFormat(JSON_TREE_KEY, json));
            }
        }

        public void Refresh()
        {
            IEnumerable<Item> items = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                items = db.LoadAll<Item>().ToList().Where(x => !x.IsDeleted);
            }
            //추가 ..
            foreach (var item in items)
            {
                bool result = Nodes.Any(n => n.Descendants().OfType<ItemNode>().Any(x => ((ItemNode)x).ItemUUID == item.UUID));
                if (!result)
                    AddNewItemInNodes(item.UUID);
            }
        }

        public static InventoryFinderViewModel CreateInventoryFinderViewModel()
        {
            ObservableCollection<DirectoryNode> nodes = null;
            SimpleStringFormat ssf = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                ssf = db.LoadByKey<SimpleStringFormat>(JSON_TREE_KEY);
            }
            if (ssf != null)
                nodes = JsonConvert.DeserializeObject<ObservableCollection<DirectoryNode>>(ssf.Data);
            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
            if (nodes != null)
                viewModel.Nodes = nodes;
            viewModel.Refresh();
            return viewModel;
        }
    }
}