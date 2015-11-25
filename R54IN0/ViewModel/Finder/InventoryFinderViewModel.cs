using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Controls.DragNDrop;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0
{
    public class InventoryFinderViewModel
    {
        DragCommand _dragCommand;
        DropCommand _dropCommand;

        public ObservableCollection<IFinderNode> Nodes { get; set; }

        public ObservableCollection<IFinderNode> SelectedNodes { get; set; }

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

        public InventoryFinderViewModel()
        {
            Nodes = new ObservableCollection<IFinderNode>();
            SelectedNodes = new ObservableCollection<IFinderNode>();
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
                if (SelectedNodes.Contains(itemToUnselect as IFinderNode))
                    SelectedNodes.Remove(itemToUnselect as IFinderNode);
            }

            foreach (object itemToSelect in e.ItemsToSelect)
            {
                if (!SelectedNodes.Contains(itemToSelect as IFinderNode))
                    SelectedNodes.Add(itemToSelect as IFinderNode);
            }
        }

        public void AddNewDirectoryInSelectedDirectory()
        {
            if (SelectedNodes.Count == 0)
                return;
            foreach (var node in SelectedNodes)
            {
                if (node.GetType() == typeof(DirectoryNode))
                {
                    node.Nodes.Add(new DirectoryNode("New Directory"));
                    break;
                }
            }
        }

        internal void RemoveNodeInRoot(IFinderNode rnode)
        {
            foreach (var d2Node in Nodes)
            {
                IEnumerable<IFinderNode> where = d2Node.Descendants().Where(n => n.Nodes.Contains(rnode));
                if (where.Count() != 0)
                    where.First().Nodes.Remove(rnode);
            }
        }

        public void DeleteSelectedDirectories()
        {
            ObservableCollection<IFinderNode> sellectNodesCopy = new ObservableCollection<IFinderNode>(SelectedNodes);
            //tree구조의 노드를 일차Collection으로 모운 뒤, itemnode를 찾아서 ROOT노드에 중복 없이 추가
            foreach (IFinderNode select in sellectNodesCopy)
            {
                IEnumerable<IFinderNode> itemNodes = select.Descendants().Where(node => node.GetType() == typeof(ItemNode));
                foreach (IFinderNode itemnode in itemNodes)
                {
                    if (Nodes.All(node => node.UUID != itemnode.UUID))
                        Nodes.Add(new ItemNode(itemnode as ItemNode));
                }
            }
            //부모노드에서 삭제
            foreach (IFinderNode selectNode in sellectNodesCopy)
                RemoveNodeInRoot(selectNode);
            SelectedNodes.Clear();
        }
    }
}