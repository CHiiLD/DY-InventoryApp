using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class ProductManagerViewModel : MultiSelectTreeViewModelView
    {
        public ICommand NewFolderAddCommand;
        public ICommand NewProductAddCommand;
        public ICommand NodeDeleteCommand;

        public ProductManagerViewModel()
        {
            Func<object, bool> returnTrue = (object obj) => { return true; };
            NewFolderAddCommand = new CommandHandler(ExecuteNewFolderAdd, returnTrue);
            NewProductAddCommand = new CommandHandler(ExecuteNewProductAdd, returnTrue);
            NodeDeleteCommand = new CommandHandler(ExecuteSelectedNodesDelete, CanDelete);
        }

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
            CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(newProduct);

            IEnumerable<TreeViewNode> folderNodes = SelectedNodes.Where(x => x.Type == NodeType.FORDER);
            TreeViewNode newTreeViewNode = new TreeViewNode(NodeType.PRODUCT, newProduct.ID);
            if (folderNodes.Count() != 0)
                Add(folderNodes.First(), newTreeViewNode);
            else
                Add(newTreeViewNode);
        }

        public bool CanDelete(object parameter)
        {
            if (SelectedNodes.Count == 0)
                return false;
            return true;
        }

        public void ExecuteSelectedNodesDelete(object parameter)
        {
            //삭제할 Node들 중에서 Product Node만 추출
            IEnumerable<TreeViewNode> productNodes = SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT)).Select(x => new TreeViewNode(NodeType.PRODUCT, x.ProductID));
            productNodes = productNodes.Distinct(); //중복 제거

            //ObservableFieldDirector의 기록 삭제
            var ofd = ObservableFieldDirector.GetInstance();
            foreach (var productNode in productNodes)
                ofd.Remove<Product>(productNode.ProductID);

            //삭제 
            foreach (var node in SelectedNodes)
                Remove(node);

            SelectedNodes.Clear();
        }
    }
}