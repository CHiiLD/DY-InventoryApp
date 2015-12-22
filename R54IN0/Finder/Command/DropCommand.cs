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
        private MultiSelectFinderViewModel _viewModel;

        public event EventHandler CanExecuteChanged;

        public DropCommand(MultiSelectFinderViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object args)
        {
            DropParameters dropParameters = args as DropParameters;
            TreeViewExItem treeviewExItem = dropParameters.DropToItem;
            IDataObject dataObject = dropParameters.DropData as IDataObject;
            FinderNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as FinderNode; //넣고자 할 장소(목적지)
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
                    FinderNode srcNode = item as FinderNode;
                    //자기노트에 자기자신을 하위로 다시 추가하는 것을 차단
                    if (destNode == srcNode)
                        return false;
                    //자신의 부모에 이미 자식으로 있음에도 다시 그 부모에 자기 자신을 추가하는 경우를 차단
                    if (destNode.Nodes.Any(x => x == srcNode))
                        return false;
                    //부모의 하위 자식에게 부모를 추가하는 경우를 차단
                    if (srcNode.Descendants().Any(x => x == destNode))
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
            FinderNode destNode = (treeviewExItem == null) ? null : treeviewExItem.DataContext as FinderNode;
            foreach (string fmt in dataObject.GetFormats())
            {
                DragContent dragContent = dataObject.GetData(fmt) as DragContent;
                if (dragContent != null)
                {
                    foreach (var selectedNode in dragContent.Items.Reverse())
                    {
                        FinderNode node = (FinderNode)selectedNode;
                        _viewModel.Director.Remove(node);
                        AddNode(destNode, index, node);
                    }
                }
            }
        }

        private void AddNode(FinderNode node, int index, FinderNode newNode)
        {
            ObservableCollection<FinderNode> children = (node == null) ? _viewModel.Nodes : node.Nodes;
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
    }
}