using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

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

        private IEnumerable<DirectoryNode> FindParentNodes(IEnumerable<DirectoryNode> root, DirectoryNode node)
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

            IEnumerable<DirectoryNode> parent = FindParentNodes(root.Nodes, node22);
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

        [Ignore]
        [TestMethod]
        public void SaveByjsonFormatTest()
        {
            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
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
            viewModel.Nodes.Add(root);

            string json = JsonConvert.SerializeObject(viewModel);
            InventoryFinderViewModel newViewModel = JsonConvert.DeserializeObject<InventoryFinderViewModel>(json);

            Assert.AreEqual(viewModel.Nodes.First().Descendants().Count(), newViewModel.Nodes.First().Descendants().Count());
        }

        [TestMethod]
        public void AddRemoveItemNodeTest()
        {
            DummyDbData dummy = new DummyDbData();
            dummy.Create();
            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
            Item item = DatabaseDirector.GetDbInstance().LoadAll<Item>()[0];

            viewModel.AddNewItemInNodes(item.UUID);
            viewModel.AddNewItemInNodes(DatabaseDirector.GetDbInstance().LoadAll<Item>()[1].UUID);
            viewModel.AddNewItemInNodes(DatabaseDirector.GetDbInstance().LoadAll<Item>()[2].UUID);

            Assert.AreEqual(item.Name, viewModel.Nodes.First().Name);
            viewModel.RemoveItemInNodes(item.UUID);
            Assert.AreEqual(2, viewModel.Nodes.Count());
        }

        [TestMethod]
        public void SaveLoadVIewModelNodes()
        {
            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
            DirectoryNode root1 = new DirectoryNode("ROOT1");
            DirectoryNode root2 = new DirectoryNode("root2");
            DirectoryNode root12 = new DirectoryNode("root2");
            DirectoryNode root112 = new DirectoryNode("root2");
            DirectoryNode root1112 = new DirectoryNode("root2");
            DirectoryNode root11112 = new DirectoryNode("root2");
            viewModel.Nodes.Add(root1);
            viewModel.Nodes.Add(root2);
            root1.Nodes.Add(root12);
            root12.Nodes.Add(root112);
            root112.Nodes.Add(root1112);
            root1112.Nodes.Add(root11112);

            viewModel.SaveTree();
            var newViewModel = InventoryFinderViewModel.CreateInventoryFinderViewModel();

            Assert.AreEqual(2 + DatabaseDirector.GetDbInstance().LoadAll<Item>().Count(), newViewModel.Nodes.Count());
        }

        [TestMethod]
        public void RefreshTest()
        {
            DummyDbData dummy = new DummyDbData();
            dummy.Create();

            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
            viewModel.Refresh();

            Assert.AreNotEqual(0, viewModel.Nodes.Count());
            Assert.AreEqual(DatabaseDirector.GetDbInstance().LoadAll<Item>().Count(), viewModel.Nodes.Count());
        }

        [Ignore]
        [TestMethod]
        public void LinqOfTypeTest()
        {
            DummyDbData dummy = new DummyDbData();
            dummy.Create();
            Item item = DatabaseDirector.GetDbInstance().LoadAll<Item>()[0];
            DirectoryNode root1 = new DirectoryNode("ROOT1");
            ItemNode root2 = new ItemNode(item.UUID);
            List<DirectoryNode> d = new List<DirectoryNode>();
            d.Add(root1);
            d.Add(root2);

            var newList = d.OfType<ItemNode>();
            Assert.AreEqual(1, newList.Count());
        }

        [Ignore]
        [TestMethod]
        public void SingleOrDefaultTest()
        {
            List<DirectoryNode> d = new List<DirectoryNode>();
            Assert.AreEqual(null, d.SingleOrDefault());
            DirectoryNode root1 = new DirectoryNode("ROOT1");
            //DirectoryNode root2 = new DirectoryNode("ROOT2");
            d.Add(root1);
            //d.Add(root2);
            Assert.AreEqual(root1, d.SingleOrDefault());
        }

        [TestMethod]
        public void DescendantsTest()
        {
            InventoryFinderViewModel viewModel = new InventoryFinderViewModel();
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
            viewModel.Nodes.Add(root);

            var result = viewModel.Nodes.SelectMany(x => x.Descendants());
            Assert.AreEqual(8, result.Count());
        }


        interface inter
        {
            int i { get; set; }
        }

        class clazz : inter
        {
            public int i { get; set; }
        }

        class box
        {
            public clazz zz { get; set; }
        }

        [TestMethod]
        public void JsonLibTest()
        {
            clazz z = new clazz();
            z.i = 10;
            string json = JsonConvert.SerializeObject(z);
            clazz newZ = JsonConvert.DeserializeObject<clazz>(json);

            Assert.AreEqual(z.i, newZ.i);

            var box = new box();
            box.zz = newZ;

            json = JsonConvert.SerializeObject(box);
            box newbox = JsonConvert.DeserializeObject<box>(json);

            Assert.AreEqual(box.zz.i, newbox.zz.i);
        }
    }
}

