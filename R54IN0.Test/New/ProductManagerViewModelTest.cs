using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Linq;

namespace R54IN0.Test.New
{
    [TestClass]
    public class ProductManagerViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new ProductManagerViewModel();
        }

        /// <summary>
        /// 새로운 폴더를 추가
        /// </summary>
        [TestMethod]
        public void CreateNewFolder()
        {
            new Dummy2().Create();
            var viewmodel = new ProductManagerViewModel();
            //새로운 폴더를 생성
            viewmodel.NewFolderAddCommand.Execute(null);
            Assert.IsTrue(viewmodel.Root.Any(node => node.Name == "새로운 폴더"));
            //그 폴더에 다시 새로운 폴더를 생성
            var newNode = viewmodel.Root.Where(node => node.Name == "새로운 폴더").Single();
            viewmodel.SelectedNodes.Add(newNode);

            viewmodel.NewFolderAddCommand.Execute(null);
            Assert.IsTrue(newNode.Root.First().Name == "새로운 폴더");
        }

        /// <summary>
        /// 새로운 제품을 추가
        /// </summary>
        [TestMethod]
        public void CreateNewProduct()
        {
            new Dummy2().Create();
            var viewmodel = new ProductManagerViewModel();
            //새로운 제품을 추가
            viewmodel.NewProductAddCommand.Execute(null);
            Assert.IsTrue(viewmodel.Root.Any(node => node.Name == "새로운 제품"));
            var newNode = viewmodel.Root.Where(node => node.Name == "새로운 제품").Single();

            //ObservableFieldDirector에 추가되었는지 확인
            var result = ObservableFieldDirector.GetInstance().Search<Product>(newNode.ProductID);
            Assert.IsNotNull(result);

            ////이름을 변경
            //newNode.IsNameEditable = true;
            //newNode.Name = "제품A";

            ////동기화 확인
            //Assert.AreEqual(newNode.Name, result.Name);
        }

        /// <summary>
        /// 기존의 폴더와 제품을 삭제
        /// </summary>
        [TestMethod]
        public void DeleteForderNProduct()
        {
            new Dummy2().Create();
            var viewmodel = new ProductManagerViewModel();
            var isvm = new InventoryStatusViewModel();
            //새로운 폴더를 생성
            viewmodel.NewFolderAddCommand.Execute(null);
            var newNode = viewmodel.Root.Where(node => node.Name == "새로운 폴더").Single();
            viewmodel.SelectedNodes.Add(newNode);
            //생성한 폴더 삭제
            viewmodel.SelectedNodes.Add(newNode);
            viewmodel.NodeDeleteCommand.Execute(null);

            Assert.IsFalse(viewmodel.Root.Any(node => node.Name == "새로운 제품"));

            //제품 하나를 선택해서 
            newNode = viewmodel.Root.Random();
            viewmodel.SelectedNodes.Add(newNode);
            //삭제 
            viewmodel.NodeDeleteCommand.Execute(null);

            Assert.IsFalse(viewmodel.Root.Any(node => node == newNode));
        }
    }
}
