using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryFinderViewModelTest
    {
        [TestMethod]
        public void CanCreateInventoryFinder()
        {
            InventoryFinderViewModel finder = new InventoryFinderViewModel();
        }

        [TestMethod]
        public void CanCreateDirectoryNode()
        {
            DirectoryNode node = new DirectoryNode();
        }

        [TestMethod]
        public void CanCreateItemNode()
        {
            ItemNode node = new ItemNode();
        }

        public InventoryFinderViewModel GetInventoryFinderViewModel()
        {
            InventoryFinderViewModel finder = new InventoryFinderViewModel();
            DirectoryNode root = new DirectoryNode();
            DirectoryNode node1 = new DirectoryNode();
            DirectoryNode node2 = new DirectoryNode();
            ItemNode itemNode = new ItemNode();
            node1.Nodes.Add(itemNode);
            root.Nodes.Add(node1);
            root.Nodes.Add(node2);
            finder.Nodes.Add(root);
            return finder;
        }

        [TestMethod]
        public void NodesTest()
        {
            var viewModel = GetInventoryFinderViewModel();
            Assert.AreEqual(1, viewModel.Nodes.Count);
        }

        private IEnumerable<IFinderNode> FindParentNodes(IEnumerable<IFinderNode> root, IFinderNode node)
        {
            foreach (var i in root)
            {
                if (i == node)
                    return root;
                if (i.Nodes != null && i.Nodes.Count != 0)
                {
                    var r = FindParentNodes(i.Nodes, node);
                    if (r != null)
                        return r;
                }
            }
            return null;
        }

        [TestMethod]
        public void RemoveNodeTest()
        {
            DirectoryNode root = new DirectoryNode("ROOT");
            DirectoryNode node1 = new DirectoryNode("ROOT_NODE1");
            DirectoryNode node2 = new DirectoryNode("ROOT_NODE2");
            DirectoryNode node21 = new DirectoryNode("ROOT_NODE2");
            DirectoryNode node22 = new DirectoryNode("ROOT_NODE2");
            DirectoryNode node11 = new DirectoryNode("ROOT_NODE2");
            DirectoryNode node12 = new DirectoryNode("ROOT_NODE2");
            DirectoryNode node121 = new DirectoryNode("ROOT_NODE2");

            root.Nodes.Add(node1);
            root.Nodes.Add(node2);
            node1.Nodes.Add(node11);
            node1.Nodes.Add(node12);
            node12.Nodes.Add(node121);
            node2.Nodes.Add(node21);
            node2.Nodes.Add(node22);

            IEnumerable<IFinderNode> parent = FindParentNodes(root.Nodes, node22);
            Assert.AreEqual(parent, node2.Nodes);
        }

        [TestMethod]
        public void FinderViewModelDirectoryAddDeleteTest()
        {
            InventoryFinderViewModel view = new InventoryFinderViewModel();
            DirectoryNode root = new DirectoryNode("ROOT");
            DirectoryNode node1 = new DirectoryNode("node1");
            DirectoryNode node2 = new DirectoryNode("node2");
            DirectoryNode node21 = new DirectoryNode("node21");
            DirectoryNode node22 = new DirectoryNode("node22");
            DirectoryNode node11 = new DirectoryNode("node11");
            DirectoryNode node12 = new DirectoryNode("node12");
            DirectoryNode node121 = new DirectoryNode("node121");

            root.Nodes.Add(node1);
            root.Nodes.Add(node2);
            node1.Nodes.Add(node11);
            node1.Nodes.Add(node12);
            node12.Nodes.Add(node121);
            node2.Nodes.Add(node21);
            node2.Nodes.Add(node22);

            view.Nodes.Add(root);
            view.SelectedNodes.Add(node121);
            view.AddNewDirectoryInSelectedDirectory();
            view.AddNewDirectoryInSelectedDirectory();
            Assert.AreEqual(node121.Nodes.Count, 2);
            view.DeleteSelectedDirectories();
            Assert.AreEqual(node12.Nodes.Count, 0);
        }

        [TestMethod]
        public void WhenDeleteDirectoryThatHasItemNodeChildrenThenItemNodeIsSurvive()
        {
            DummyDbData db = new DummyDbData();
            InventoryFinderViewModel view = new InventoryFinderViewModel();
            DirectoryNode root = new DirectoryNode("ROOT");
            DirectoryNode node1 = new DirectoryNode("node1");
            DirectoryNode node2 = new DirectoryNode("node2");
            DirectoryNode node21 = new DirectoryNode("node21");
            DirectoryNode node22 = new DirectoryNode("node22");
            DirectoryNode node11 = new DirectoryNode("node11");
            DirectoryNode node12 = new DirectoryNode("node12");
            ItemNode itemNode = new ItemNode(DatabaseDirector.GetDbInstance().LoadAll<Item>()[0].UUID);

            root.Nodes.Add(node1);
            root.Nodes.Add(node2);
            node1.Nodes.Add(node11);
            node1.Nodes.Add(node12);
            node12.Nodes.Add(itemNode);
            node2.Nodes.Add(node21);
            node2.Nodes.Add(node22);

            view.Nodes.Add(root);
            view.SelectedNodes.Add(node12);
            view.DeleteSelectedDirectories();

            Assert.IsTrue(view.Nodes.Any(node => node.UUID == itemNode.UUID));
        }
    }
}
