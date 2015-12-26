﻿using System;
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
using System.Diagnostics;

namespace R54IN0
{
    public class ItemFinderViewModel : FinderViewModel
    {
        FinderDirector _finderDirector;

        public ItemFinderViewModel(TreeViewEx treeView) : base(treeView)
        {
            DragCommand = new DragCommand();
            DropCommand = new DropCommand(this);

            RemoveDirectoryCommand = new CommandHandler(RemoveSelectedDirectories, CanRemoveSelectedDirectoies);
            AddNewDirectoryCommand = new CommandHandler(AddNewDirectories, CanAddNewDirectory);

            _finderDirector = FinderDirector.GetInstance();
        }

        internal FinderDirector Director
        {
            get
            {
                return _finderDirector;
            }
        }

        public override ObservableCollection<FinderNode> Nodes
        {
            get
            {
                return _finderDirector.Collection;
            }
        }

        public DragCommand DragCommand { get; private set; }
        public DropCommand DropCommand { get; private set; }

        public CommandHandler AddNewDirectoryCommand { get; private set; }
        public CommandHandler RemoveDirectoryCommand { get; private set; }

        public bool CanAddNewDirectory(object pamateter)
        {
            return true;
        }

        public void AddNewDirectories(object parameter)
        {
            IEnumerable<FinderNode> directoryNodeInSelection = SelectedNodes.Where(x => x.Type == NodeType.DIRECTORY);
            var newNode = new FinderNode(NodeType.DIRECTORY) { Name = "New Directory" };
            if (directoryNodeInSelection.Count() != 0)
                _finderDirector.Add(directoryNodeInSelection.First(), newNode);
            else
                _finderDirector.Add(newNode);
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
            var itemNodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.ITEM)).Select(x => new FinderNode(x));
            itemNodes = new List<FinderNode>(itemNodes); //논리적 이유는 추측하기 어려우나(WPF버그?), 이 코드가 없으면 논리에러가 발생!
#if DEBUG
            Debug.Assert(!itemNodes.Any(x => Nodes.SelectMany(n => n.Descendants()).Any(n2 => n2 == x)));
            int cnt = itemNodes.Count();
#endif
            foreach (var node in new List<FinderNode>(SelectedNodes))
                _finderDirector.Remove(node);
#if DEBUG
            Debug.Assert(cnt == itemNodes.Count());
#endif
            foreach (var itemNode in itemNodes)
                _finderDirector.Add(itemNode);
            SelectedNodes.Clear();
        }

        /// <summary>
        /// TreeViewEx의 OnSelecting 이벤트와 연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnNodeSelected(object sender, SelectionChangedCancelEventArgs e)
        {
            base.OnNodeSelected(sender, e);
            RemoveDirectoryCommand.UpdateCanExecute();
        }
    }
}