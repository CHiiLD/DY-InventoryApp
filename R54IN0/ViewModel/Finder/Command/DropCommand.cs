using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.DragNDrop;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace R54IN0
{
    public class DropCommand : ICommand
    {
        private InventoryFinderViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public DropCommand(InventoryFinderViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void AddNode(DirectoryNode node, int index, DirectoryNode newNode)
        {
            ObservableCollection<DirectoryNode> children = (node == null) ? _viewModel.Nodes : node.Nodes;
            if (index == -1)
                children.Add(newNode); //drop
            else
                children.Insert(index, newNode); //insert
        }

        public bool CanExecute(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            DirectoryNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as DirectoryNode; //넣고자 할 장소(목적지)
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
                    DirectoryNode directoryNode = item as DirectoryNode;
                    //자기노트에 자기자신을 하위로 다시 추가하는 것을 차단
                    if (destNode == directoryNode)
                        return false;
                    //자신의 부모에 이미 자식으로 있음에도 다시 그 부모에 자기 자신을 추가하는 경우를 차단
                    foreach (DirectoryNode nodeChild in destNode.Nodes)
                    {
                        if (nodeChild == directoryNode)
                            return false;
                    }
                    //부모가 자기 자신의 자식안에 추가하는 경우를 차단 (모순)
                    var find = directoryNode.Descendants().Where(node => node == destNode);
                    if (find.Count() != 0)
                        return false;
                }
            }
            if (index == -1)
                return destNode.AllowDrop;
            else
                return destNode.AllowInsert;
        }

        /// <summary>
        /// 드랍 명령의 실행 기능 메서드
        /// </summary>
        /// <param name="args"></param>
        public void Execute(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            int index = dropParameters.Index;
            DirectoryNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as DirectoryNode;
            foreach (string fmt in dataObject.GetFormats())
            {
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                if (dragContent != null)
                {
                    foreach (var selectedNode in dragContent.Items.Reverse())
                    {
                        DirectoryNode oldNode = (DirectoryNode)selectedNode;
                        DirectoryNode newNode = null;
                        if (oldNode.GetType() == typeof(DirectoryNode))
                            newNode = new DirectoryNode(oldNode as DirectoryNode);
                        else if (oldNode.GetType() == typeof(ItemNode))
                            newNode = new ItemNode(oldNode as ItemNode);
                        _viewModel.RemoveNodeInRoot(oldNode);
                        AddNode(destNode, index, newNode);
                    }
                }
            }
        }
    }
}