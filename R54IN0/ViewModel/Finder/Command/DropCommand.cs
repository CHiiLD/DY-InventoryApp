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

        private void AddNode(IFinderNode node, int index, IFinderNode newNode)
        {
            ObservableCollection<IFinderNode> children = (node == null) ? _viewModel.Nodes : node.Nodes;
            if (index == -1)
                children.Add(newNode); //drop
            else
                children.Insert(index, newNode); //insert
        }

        //private Collection<IFinderNode> FindParentNodes(Collection<IFinderNode> root, IFinderNode node)
        //{
        //    foreach (var i in root)
        //    {
        //        if (i == node)
        //            return root;
        //        if (i.Nodes != null && i.Nodes.Count != 0)
        //        {
        //            var r = FindParentNodes(i.Nodes, node);
        //            if (r != null)
        //                return r;
        //        }
        //    }
        //    return null;
        //}

        //private void RemoveNode(IFinderNode node)
        //{
        //    var parent = FindParentNodes(_viewModel.Nodes, node);
        //    Debug.Assert(parent != null);
        //    parent.Remove(node);
        //}

        /// <summary>
        /// DROP 가능 여부를 알린다. 가불가에 따라 마우스 포인터가 달라진다.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>가능 여부</returns>
        public bool CanExecute(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            if (treeviewExItem == null)
                return true; //drop to root
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            IFinderNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as IFinderNode; //넣고자 할 장소(목적지)
            int index = dropParameters.Index;

            foreach (string fmt in dataObject.GetFormats())
            {
                //외부의 드래그를 차단
                if (!(dataObject.GetData(fmt) is DragContent))
                    return false;
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                foreach (object item in dragContent.Items)
                {
                    //자기노트에 자기자신을 하위로 다시 추가하는 것을 차단
                    if (destNode == item)
                        return false;

                    //자신의 부모에 이미 자식으로 있음에도 다시 그 부모에 자기 자신을 추가하는 경우를 차단
                    foreach (IFinderNode nodeChild in destNode.Nodes)
                    {
                        if (nodeChild == item)
                            return false;
                    }
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
            IFinderNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as IFinderNode;
            foreach (string fmt in dataObject.GetFormats())
            {
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                if (dragContent != null)
                {
                    foreach (var selectedNode in dragContent.Items.Reverse())
                    {
                        IFinderNode oldNode = (IFinderNode)selectedNode;
                        IFinderNode newNode = null;
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