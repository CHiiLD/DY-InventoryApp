﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class ProductManagerViewModel : MultiSelectTreeViewModelView
    {
        public ProductManagerViewModel()
        {
            Func<object, bool> returnTrue = (object obj) => { return true; };
            NewFolderAddCommand = new CommandHandler(ExecuteNewFolderAdd, returnTrue);
            NewProductAddCommand = new CommandHandler(ExecuteNewProductAdd, returnTrue);
            NodeDeleteCommand = new CommandHandler(ExecuteSelectedNodesDelete, CanDelete);
        }

        /// <summary>
        /// 새로운 폴더 추가 
        /// </summary>
        public ICommand NewFolderAddCommand { get; set; }

        /// <summary>
        /// 새로운 제품 추가 
        /// </summary>
        public ICommand NewProductAddCommand { get; set; }

        /// <summary>
        /// 선택된 폴더 삭제
        /// </summary>
        public ICommand NodeDeleteCommand { get; set; }

        public void ExecuteNewFolderAdd(object parameter)
        {
            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FORDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(NodeType.FORDER, "새로운 폴더");
            if (folderNodes.Count() != 0)
                Add(folderNodes.First(), newTreeViewNode);
            else
                Add(newTreeViewNode);
        }

        /// <summary>
        /// 새로운 Product 객체를 생성하여 ProductManager에 추가
        /// </summary>
        /// <param name="parameter"></param>
        public void ExecuteNewProductAdd(object parameter)
        {
            Product product = new Product() { Name = "새로운 제품" }.Save<Product>();
            Observable<Product> newProduct = new Observable<Product>(product);
            ObservableFieldDirector.GetInstance().Add<Product>(newProduct);
            //CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(newProduct);

            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FORDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(NodeType.PRODUCT, newProduct.ID);
            if (folderNodes.Count() != 0)
                Add(folderNodes.First(), newTreeViewNode);
            else
                Add(newTreeViewNode);
        }

        public override void OnNodeSelected(object sender, SelectionChangedCancelEventArgs e)
        {
            base.OnNodeSelected(sender, e);
            var cmd = NodeDeleteCommand as CommandHandler;
            cmd.UpdateCanExecute();
        }

        //public bool CanDelete(object parameter)
        //{
        //    if (SelectedNodes.Count == 0)
        //        return false;
        //    return true;
        //}

        //public void ExecuteSelectedNodesDelete(object parameter)
        //{
        //    //삭제할 Node들 중에서 Product Node만 추출
        //    IEnumerable<TreeViewNode> productNodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Select(x => new TreeViewNode(NodeType.PRODUCT, x.ProductID));
        //    productNodes = productNodes.Distinct(); //중복 제거

        //    //ObservableFieldDirector의 기록 삭제
        //    var ofd = ObservableFieldDirector.GetInstance();
        //    foreach (var productNode in productNodes)
        //        ofd.Remove<Product>(productNode.ProductID);

        //    //삭제 
        //    foreach (var node in new List<TreeViewNode>(SelectedNodes))
        //        Remove(node);

        //    SelectedNodes.Clear();
        //}

        /// <summary>
        /// 폴더 삭제 허용 여부 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanDelete(object parameter)
        {
            if (SelectedNodes.Count == 0)
                return false;
            if (SelectedNodes.Any(x => x.Type == NodeType.PRODUCT))
                return false;
            return true;
        }

        /// <summary>
        /// 선택된 노드 삭제
        /// </summary>
        /// <param name="parameter"></param>
        public void ExecuteSelectedNodesDelete(object parameter)
        {
            var productNodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Select(x => new TreeViewNode(NodeType.PRODUCT, x.ProductID));
            productNodes = new List<TreeViewNode>(productNodes); //논리적 이유는 추측하기 어려우나(WPF버그?), 이 코드가 없으면 논리에러가 발생!
            foreach (var node in new List<TreeViewNode>(SelectedNodes))
                Remove(node);
            foreach (var itemNode in productNodes)
                Add(itemNode);
            SelectedNodes.Clear();
        }
    }
}