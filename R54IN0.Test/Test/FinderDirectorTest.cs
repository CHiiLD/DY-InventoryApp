using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class FinderDirectorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            FinderDirector df = FinderDirector.GetInstance();
        }

        /// <summary>
        /// DN -
        ///   - Item1N - item11N
        ///   - Item2N 
        /// 구조에서 DN을 삭제할 경우 아래 item들은 전부 root에 넣어버린다.
        /// </summary>
        [TestMethod]
        public void DeleteParentNode()
        {
            new DummyDbData().Create();
            FinderDirector df = FinderDirector.GetInstance();
            var collection = df.Collection;
            collection.Clear();
            FinderNode root = new FinderNode(NodeType.DIRECTORY) { Name = "D" };
            FinderNode item1 = new FinderNode(NodeType.ITEM) { Name = "I1", ItemID = "1" };
            FinderNode item2 = new FinderNode(NodeType.ITEM) { Name = "I2", ItemID = "2" };
            FinderNode item11 = new FinderNode(NodeType.ITEM) { Name = "I11", ItemID = "3" };
            root.Nodes.Add(item1);
            root.Nodes.Add(item2);
            item1.Nodes.Add(item11);
            collection.Add(root);

            ItemFinderViewModel fvm = new ItemFinderViewModel(null);
            fvm.SelectedNodes.Clear();
            fvm.SelectedNodes.Add(root);
            fvm.RemoveDirectoryCommand.Execute(null);

            Assert.AreEqual(3, collection.Count);
            Assert.IsTrue(collection.Any(x => x.Name == item1.Name));
            Assert.IsTrue(collection.Any(x => x.Name == item2.Name));
            Assert.IsTrue(collection.Any(x => x.Name == item11.Name));
        }
    }
}
