using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InventorySearchTextBoxViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new InventorySearchTextBoxViewModel();
        }

        [TestMethod]
        public void Search()
        {
            new Dummy2().Create();
            string product = "     스위치 ";
            string dummyName = "23094832098432";
            string somethingName = "버튼\t 단자부\n버섯\r 213o4u12oi\t";
            string specName = "KCB";

            var vm = new InventorySearchTextBoxViewModel();
            
            //제품 이름 검색 
            vm.Text = product;
            var result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            Assert.IsTrue(result.All(x => x.Product.Name.Contains("스위치")));
            
            //더미로 검색하였을 때 검색 실패
            vm.Text = dummyName;
            result = vm.Search();

            Assert.AreEqual(0, result.Count());
            
            //여러개의 키워드를 넣었을 경우 or연산으로 쿼리
            vm.Text = somethingName;
            result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            foreach (var name in somethingName.Split(new char[] { ' ', '\t', '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries))
            {
                Assert.IsTrue
                    (
                        result.Any(x => x.Product.Name.Contains("버튼")) ||
                        result.Any(x => x.Product.Name.Contains("단자부")) ||
                        result.Any(x => x.Product.Name.Contains("버섯"))
                    );
            }

            //규격으로 검색
            vm.Text = specName;
            result = vm.Search();

            Assert.AreNotEqual(0, result.Count());
            Assert.IsTrue(result.All(x => x.Product.Name.Contains("SWTICH")));
        }
    }
}