using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace R54IN0.WPF
{
    /// <summary>
    /// MultiSelectTreeView ViewModel 클래스
    /// </summary>
    public partial class MultiSelectTreeViewModelView
    {
        /// <summary>
        /// 노드 마우스 우측 클릭 이벤트
        /// </summary>
        public RelayCommand<MouseButtonEventArgs> MouseRightButtonDownEventCommand { get; private set; }

        /// <summary>
        /// 노드 마우스 좌측 클릭으로 인한 선택 이벤트
        /// </summary>
        public RelayCommand<SelectionChangedCancelEventArgs> NodesSelectedEventCommand { get; private set; }

        /// <summary>
        /// 이름 바꾸기
        /// </summary>
        public RelayCommand SelectedNodeRenameCommand { get; private set; }

        /// <summary>
        /// 새로운 폴더 추가
        /// </summary>
        public RelayCommand NewFolderNodeAddCommand { get; private set; }

        /// <summary>
        /// 새로운 제품 추가
        /// </summary>
        public RelayCommand NewProductNodeAddCommand { get; private set; }

        /// <summary>
        /// 새로운 인벤토리 추가
        /// </summary>
        public RelayCommand NewInventoryNodeAddCommand { get; private set; }

        /// <summary>
        /// 제품별 입출고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsIOStockRecordCommand { get; private set; }

        /// <summary>
        /// 새로운 입출고 데이터 추가하기
        /// </summary>
        public RelayCommand IOStockAmenderWindowCallCommand { get; private set; }

        /// <summary>
        /// 재고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsInventoryRecordCommand { get; private set; }

        /// <summary>
        /// 선택된 노드 삭제하기
        /// </summary>
        public RelayCommand SelectedNodeDeletionCommand { get; private set; }

        

        /// <summary>
        /// 선택된 노드의 이름을 변경한다.
        /// </summary>
        private void ExecuteSelectedNodeRenameCommand()
        {
            if (SelectedNodes.Count() > 0)
                SelectedNodes.First().IsNameEditable = true;
        }

        /// <summary>
        /// 새로운 제품 노드를 추가한다.
        /// </summary>
        private async void ExecuteNewProductNodeAddCommand()
        {
            Observable<Product> newProduct = new Observable<Product>("새로운 제품");
            await InventoryDataCommander.GetInstance().AddObservableField(newProduct);

            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FOLDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(newProduct);
            if (folderNodes.Count() != 0)
                _director.AddToParent(folderNodes.First(), newTreeViewNode);
            else
                _director.AddToRoot(newTreeViewNode);
        }

        /// <summary>
        /// 새로운 폴더 노드를 추가한다.
        /// </summary>
        private void ExecuteNewFolderNodeAddCommand()
        {
            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FOLDER);
            TreeViewNode newTreeViewNode = new TreeViewNode("새로운 폴더");
            if (folderNodes.Count() != 0)
                _director.AddToParent(folderNodes.First(), newTreeViewNode);
            else
                _director.AddToRoot(newTreeViewNode);
        }

        /// <summary>
        /// IOStockDataAmenderWindow를 연다.
        /// </summary>
        private void ExecuteIOStockAmenderWindowCallCommand()
        {
            if (SelectedNodes.Count() == 1 && SelectedNodes.Single().Type == NodeType.PRODUCT)
                MainWindowViewModel.GetInstance().ShowIOStockDataAmenderWindow(SelectedNodes.Single().ObservableObjectID);
        }

        /// <summary>
        /// 선택된 제품 노드를 사용하여 제품별 입출고 현황으로 이동한다.
        /// </summary>
        private void ExecuteSearchAsIOStockRecordCommand()
        {
            if (SelectedNodes.Count() == 1)
                MainWindowViewModel.GetInstance().ShowIOStockStatusByProduct(SelectedNodes.Single().ObservableObjectID);
        }

        /// <summary>
        /// 선택된 제품 노드를 사용하여 재고 현황으로 이동한다.
        /// </summary>
        private void ExecuteSearchAsInventoryRecordCommand()
        {
            if (SelectedNodes.Count() == 1)
                MainWindowViewModel.GetInstance().ShowInventoryStatus(SelectedNodes.Single().ObservableObjectID);
        }

        /// <summary>
        /// 마우스 우측클릭으로 노드를 선택했을 경우 그 노드만 SelectedNodes에 추가한다.
        /// </summary>
        /// <param name="e"></param>
        private void ExecuteMouseRightButtonDownEventCommand(MouseButtonEventArgs e)
        {
            Debug.WriteLine("ExecuteMouseRightButtonDownEventCommand");
            Point pt = e.GetPosition((UIElement)e.Source);
            TreeViewEx treeViewEx = e.Source as TreeViewEx;
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeViewEx, pt);
            if (hitTestResult == null || hitTestResult.VisualHit == null)
                return;
            SelectedNodes.Clear();
            FrameworkElement child = hitTestResult.VisualHit as FrameworkElement;
            do
            {
                //TreeViewNode를 선택한 경우
                if (child is TreeViewExItem && child.IsMouseOver)
                {
                    TreeViewNode node = child.DataContext as TreeViewNode;
                    if (node != null)
                        ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
                    break;
                }
                //node가 아닌 배경을 선택한 경우
                else if (child is TreeViewEx)
                    break;
                else
                    child = VisualTreeHelper.GetParent(child) as FrameworkElement;
            } while (child != null);

            SelectedNodeRenameCommand.RaiseCanExecuteChanged();
            SearchAsIOStockRecordCommand.RaiseCanExecuteChanged();
            IOStockAmenderWindowCallCommand.RaiseCanExecuteChanged();
            SearchAsInventoryRecordCommand.RaiseCanExecuteChanged();
            SelectedNodeDeletionCommand.RaiseCanExecuteChanged();

            NewFolderNodeAddCommand.RaiseCanExecuteChanged();
            NewProductNodeAddCommand.RaiseCanExecuteChanged();
            NewInventoryNodeAddCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 노드들을 선택했을 경우 호출되는 EventToCommand
        /// </summary>
        /// <param name="e"></param>
        public void ExecuteNodesSelectedEventCommand(SelectionChangedCancelEventArgs e)
        {
            Debug.WriteLine("ExecuteNodesSelectedEventCommand");
            e.Cancel = true;
            if (e.ItemsToUnSelect != null)
            {
                foreach (object itemToUnselect in e.ItemsToUnSelect)
                {
                    if (SelectedNodes.Contains(itemToUnselect as TreeViewNode))
                        SelectedNodes.Remove(itemToUnselect as TreeViewNode);
                }
            }
            if (e.ItemsToSelect != null)
            {
                foreach (object itemToSelect in e.ItemsToSelect)
                {
                    if (!SelectedNodes.Contains(itemToSelect as TreeViewNode))
                        SelectedNodes.Add(itemToSelect as TreeViewNode);
                }
            }
            NotifyPropertyChanged("SelectedNodes");
        }

        private async Task<MessageDialogResult> ShowAttentionMessage(string message)
        {
            const string title = "주의!";
            MessageDialogResult result = MessageDialogResult.Affirmative;
            if (Application.Current != null)
            {
                var metro = Application.Current.MainWindow as MetroWindow;
                result = await metro.ShowMessageAsync(
                    title, message, MessageDialogStyle.AffirmativeAndNegative,
                    new MetroDialogSettings() { AffirmativeButtonText = "네", NegativeButtonText = "아니오", ColorScheme = MetroDialogColorScheme.Accented });
            }
            return result;
        }

        /// <summary>
        /// 선택된 노드를 삭제한다.
        /// </summary>
        private async void ExecuteSelectedNodeDeletionCommand()
        {
            if (SelectedNodes.Count() != 1)
                return;
            var selectedNode = SelectedNodes.Single();
            MessageDialogResult result = MessageDialogResult.Affirmative;

            switch (selectedNode.Type)
            {
                case NodeType.FOLDER:
                    result = await ShowAttentionMessage(string.Format("{0} 폴더를 삭제합니다.\n정말로 삭제하시겠습니까?", selectedNode.Name));
                    break;
                case NodeType.PRODUCT:
                    result = await ShowAttentionMessage(string.Format("{0} 제품과 관련된 모든 재고기록과 입출고기록을 삭제합니다.\n정말로 삭제하시겠습니까?", selectedNode.Name));
                    break;
            }
            if (result != MessageDialogResult.Affirmative)
                return;

            switch (selectedNode.Type)
            {
                case NodeType.FOLDER:
                    var productNodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).ToList();
                    _director.Remove(selectedNode);
                    productNodes.ForEach(x => _director.AddToRoot(x));
                    break;
                case NodeType.PRODUCT:
                    _director.Remove(selectedNode);
                    var product = InventoryDataCommander.GetInstance().SearchObservableField<Product>(selectedNode.ObservableObjectID);
                    await InventoryDataCommander.GetInstance().RemoveObservableField(product);
                    break;
            }
            SelectedNodes.Clear();
        }

        private void ExecuteNewInventoryNodeAddCommand()
        {
        }

        private bool CanAddFolderNode()
        {
            if (!IsOnlyOne())
                return true;
            var node = SelectedNodes.Single();
            switch (node.Type)
            {
                case NodeType.FOLDER:
                    return true;
                case NodeType.PRODUCT:
                case NodeType.INVENTORY:
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }

        private bool CanAddProductNode()
        {
            if (!IsOnlyOne())
                return true;
            var node = SelectedNodes.Single();
            switch (node.Type)
            {
                case NodeType.FOLDER:
                    return true;
                case NodeType.PRODUCT:
                case NodeType.INVENTORY:
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }

        private bool CanAddInventoryNode()
        {
            if (!IsOnlyOne())
                return false;
            var node = SelectedNodes.Single();
            switch (node.Type)
            {
                case NodeType.FOLDER:
                    return false;
                case NodeType.PRODUCT:
                    return true;
                case NodeType.INVENTORY:
                    return false;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 선택된 노드가 제품 노드인지 파악
        /// </summary>
        /// <returns></returns>
        private bool IsProductNode()
        {
            if (IsOnlyOne())
            {
                var node = SelectedNodes.Single();
                return node.Type == NodeType.PRODUCT;
            }
            return false;
        }

        /// <summary>
        /// 선택된 노드들이 하나뿐인지 파악
        /// </summary>
        /// <returns></returns>
        private bool IsOnlyOne()
        {
            Debug.WriteLine("IsOnlyOne cnt: " + SelectedNodes.Count());
            return SelectedNodes.Count() == 1;
        }

        public bool CanDeleteNode()
        {
            return SelectedNodes.Count() == 1;
        }
    }
}