using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
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
        public RelayCommand<MouseButtonEventArgs> MouseRightButtonDownEventCommand { get; set; }

        /// <summary>
        /// 노드 마우스 좌측 클릭으로 인한 선택 이벤트
        /// </summary>
        public RelayCommand<SelectionChangedCancelEventArgs> NodesSelectedEventCommand { get; set; }

        /// <summary>
        /// 이름 바꾸기
        /// </summary>
        public RelayCommand SelectedNodeRenameCommand { get; set; }

        /// <summary>
        /// 새로운 폴더 추가
        /// </summary>
        public RelayCommand NewFolderNodeAddCommand { get; set; }

        /// <summary>
        /// 새로운 제품 추가
        /// </summary>
        public RelayCommand NewProductNodeAddCommand { get; set; }

        /// <summary>
        /// 제품별 입출고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsIOStockRecordCommand { get; set; }

        /// <summary>
        /// 새로운 입출고 데이터 추가하기
        /// </summary>
        public RelayCommand IOStockAmenderWindowCallCommand { get; set; }

        /// <summary>
        /// 재고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsInventoryRecordCommand { get; set; }

        /// <summary>
        /// 선택된 노드 삭제하기
        /// </summary>
        public RelayCommand SelectedNodeDeletionCommand { get; set; }

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
        private void ExecuteNewProductNodeAddCommand()
        {
            Product product = new Product() { Name = "새로운 제품" }.Save<Product>();
            Observable<Product> newProduct = new Observable<Product>(product);
            ObservableFieldDirector.GetInstance().Add<Product>(newProduct);

            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FORDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(NodeType.PRODUCT, newProduct.ID);
            if (folderNodes.Count() != 0)
                _director.Add(folderNodes.First(), newTreeViewNode);
            else
                _director.Add(newTreeViewNode);
        }

        /// <summary>
        /// 새로운 폴더 노드를 추가한다.
        /// </summary>
        private void ExecuteNewFolderNodeAddCommand()
        {
            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FORDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(NodeType.FORDER, "새로운 폴더");
            if (folderNodes.Count() != 0)
                _director.Add(folderNodes.First(), newTreeViewNode);
            else
                _director.Add(newTreeViewNode);
        }

        /// <summary>
        /// IOStockDataAmenderWindow를 연다.
        /// </summary>
        private void ExecuteIOStockAmenderWindowCallCommand()
        {
            if (SelectedNodes.Count() == 1 && SelectedNodes.Single().Type == NodeType.PRODUCT)
                MainWindowViewModel.GetInstance().ShowIOStockDataAmenderWindow(SelectedNodes.Single().ProductID);
        }

        /// <summary>
        /// 선택된 제품 노드를 사용하여 제품별 입출고 현황으로 이동한다.
        /// </summary>
        private void ExecuteSearchAsIOStockRecordCommand()
        {
            if (SelectedNodes.Count() == 1)
                MainWindowViewModel.GetInstance().ShowIOStockStatusByProduct(SelectedNodes.Single().ProductID);
        }

        /// <summary>
        /// 선택된 제품 노드를 사용하여 재고 현황으로 이동한다.
        /// </summary>
        private void ExecuteSearchAsInventoryRecordCommand()
        {
            if (SelectedNodes.Count() == 1)
                MainWindowViewModel.GetInstance().ShowInventoryStatus(SelectedNodes.Single().ProductID);
        }

        /// <summary>
        /// 마우스 좌측클릭으로 노드를 선택했을 경우 그 노드만 SelectedNodes에 추가한다.
        /// </summary>
        /// <param name="eventArgs"></param>
        private void ExecuteMouseRightButtonDownEventCommand(MouseButtonEventArgs eventArgs)
        {
            Point pt = eventArgs.GetPosition((UIElement)eventArgs.Source);
            TreeViewEx treeViewEx = eventArgs.Source as TreeViewEx;
            HitTestResult hitTestResult = VisualTreeHelper.HitTest(treeViewEx, pt);
            if (hitTestResult == null || hitTestResult.VisualHit == null)
                return;
            FrameworkElement child = hitTestResult.VisualHit as FrameworkElement;
            do
            {
                if (child is TreeViewExItem)
                {
                    TreeViewNode node = child.DataContext as TreeViewNode;
                    if (node != null)
                    {
                        SelectedNodes.Clear();
                        SelectedNodes.Add(node);
                    }
                    break;
                }
                else if (child is TreeViewEx)
                {
                    break;
                }
                else
                {
                    child = VisualTreeHelper.GetParent(child) as FrameworkElement;
                }
            } while (child != null);
        }

        /// <summary>
        /// 노드들을 선택했을 경우 호출되는 EventToCommand
        /// </summary>
        /// <param name="e"></param>
        public void ExecuteNodesSelectedEventCommand(SelectionChangedCancelEventArgs e)
        {
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
            SearchAsIOStockRecordCommand.RaiseCanExecuteChanged();
            IOStockAmenderWindowCallCommand.RaiseCanExecuteChanged();
            SearchAsInventoryRecordCommand.RaiseCanExecuteChanged();
            SelectedNodeDeletionCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 선택된 노드를 삭제한다.
        /// </summary>
        private void ExecuteNodeDeletionCommand()
        {
            var nodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Select(x => new TreeViewNode(NodeType.PRODUCT, x.ProductID));
            foreach (var node in nodes)
            {
                var subject = CollectionViewModelObserverSubject.GetInstance();
                var inventodyDirector = ObservableInventoryDirector.GetInstance();
                var inventoryList = inventodyDirector.CreateList();
                foreach (var inven in inventoryList)
                {
                    subject.NotifyItemDeleted(inven);
                    if (inven.Product.ID == node.ProductID)
                        inventodyDirector.Remove(inven);
                    inven.Format.Delete<InventoryFormat>();
                }
                var product = ObservableFieldDirector.GetInstance().Search<Product>(node.ProductID);
                ObservableFieldDirector.GetInstance().Remove<Product>(node.ProductID);
                product.Field.Delete<Product>();
            }
            _director.Remove(SelectedNodes.First());
            SelectedNodes.Clear();
        }

        /// <summary>
        /// 선택된 노드가 제품 노드인지 파악
        /// </summary>
        /// <returns></returns>
        private bool IsProductNode()
        {
            if (SelectedNodes.Count() == 1)
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
        private bool HasOneNode()
        {
            return SelectedNodes.Count() == 1;
        }

        public bool CanDeleteNode()
        {
            return SelectedNodes.Count() == 1;
        }
    }
}