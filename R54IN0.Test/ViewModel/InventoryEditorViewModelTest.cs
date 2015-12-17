using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.ObjectModel;

namespace R54IN0.Test
{
    [TestClass]
    public class InventoryEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateInventoryEditorViewModel()
        {
            new InventoryEditorViewModel();
        }

        [TestMethod]
        public void CanCopyInventoryEditorViewModel()
        {
            new DummyDbData().Create();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var invens = db.LoadAll<Inventory>();
                new InventoryEditorViewModel(invens.FirstOrDefault());
            }
        }

        [TestMethod]
        public void AddTest()
        {
            new DummyDbData().Create();
            var viewModel = new InventoryEditorViewModel();
            viewModel.SelectedItem = viewModel.AllItem.FirstOrDefault();
            viewModel.SelectedSpecification = viewModel.AllSpecification.FirstOrDefault();
            viewModel.ItemCount = 10;
            viewModel.SelectedWarehouse = viewModel.AllWarehouse.FirstOrDefault();

            var dgViewModel = new InventoryDataGridViewModel();
            int count = dgViewModel.Items.Count;

            //Assert.IsNotNull(viewModel.Inventory.SpecificationUUID);
            Assert.IsNotNull(viewModel.Inventory.WarehouseUUID);

            dgViewModel.AddNewItem(viewModel.Inventory);

            //Assert.AreEqual(count + 1, dgViewModel.Items.Count);

            dgViewModel = new InventoryDataGridViewModel();
            //Assert.AreEqual(count + 1, dgViewModel.Items.Count);
        }

        [TestMethod]
        public void CopySpecification()
        {
            new DummyDbData().Create();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var spec = db.LoadAll<Specification>().FirstOrDefault();
                var cpy = new Specification(spec);

                Assert.AreEqual(spec.UUID, cpy.UUID);
            }
        }

        [Ignore]
        [TestMethod]
        public void CopyInventoryPipe()
        {
            new DummyDbData().Create();
            Inventory inven;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                inven = db.LoadAll<Inventory>().FirstOrDefault();
            }
            var cpy = new Inventory(inven);

            Assert.AreEqual(inven.UUID, cpy.UUID);

            var pipe = new InventoryPipe(inven);
            Assert.AreEqual(inven.UUID, pipe.Inven.UUID);
            Assert.AreEqual(inven.SpecificationUUID, pipe.Inven.SpecificationUUID);
            Assert.AreEqual(inven.SpecificationUUID, pipe.Specification.Field.UUID);
        }

        [Ignore]
        [TestMethod]
        public void AllSpecificationTest()
        {
            new DummyDbData().Create();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                for (int i = 0; i < 20; i++)
                {
                    InventoryEditorViewModel viewModel;
                    Inventory[] invens;

                    invens = db.LoadAll<Inventory>();
                    var inven = invens.FirstOrDefault();
                    viewModel = new InventoryEditorViewModel(inven);

                    Assert.IsNotNull(viewModel.SelectedItem);
                    Assert.IsNotNull(viewModel.SelectedSpecification);
                    Assert.IsNotNull(viewModel.SelectedWarehouse);

                    var spec1 = db.LoadByKey<Specification>(viewModel.SelectedSpecification.Field.UUID);
                    var spec2 = db.LoadByKey<Specification>(inven.SpecificationUUID);

                    Assert.AreEqual(spec1.Name, spec2.Name);
                    Assert.AreEqual(viewModel.SelectedSpecification.Field.UUID, inven.SpecificationUUID);
                    Assert.AreEqual(viewModel.SelectedWarehouse.Field.UUID, inven.WarehouseUUID);

                    var uuid = viewModel.AllSpecification.FirstOrDefault().Field.UUID;
                }
            }
            //viewModel.SelectedItem = invens.Select(x => x.SpecificationUUID )
            //Assert.AreNotEqual(uuid, viewModel.AllSpecification.FirstOrDefault().UUID);
        }

        //[TestMethod]
        //public void ReplaceTest()
        //{
        //    for (int i = 0; i < 20; i++ )
        //    {
        //        new DummyDbData().Create();
        //        var dgvm = new InventoryDataGridViewModel();
        //        var sel = dgvm.SelectedItem.Inven;
        //        var cnt = dgvm.Items.Count;
        //        var viewModel = new InventoryEditorViewModel(sel);
        //        viewModel.ItemCount = 100;
        //        var inven = viewModel.Inventory;
        //        dgvm.Replace(inven);

        //        Assert.AreEqual(cnt, dgvm.Items.Count);
        //        Assert.AreEqual(100, dgvm.SelectedItem.ItemCount);
        //    }
        //}

        [Ignore]
        [TestMethod]
        public void AddTest2()
        {
            //new DummyDbData().Create();
            //var datagridViewModel = new InventoryDataGridViewModel();
            //var pip = datagridViewModel.SelectedItem;
            //var sel = datagridViewModel.SelectedItem.Inven;
            //var cnt = datagridViewModel.Items.Count;
            //var editViewModel = new InventoryEditorViewModel();
            //editViewModel.ItemCount = 10;
            //editViewModel.SelectedItem = sel.TraceItem();
            //editViewModel.SelectedSpecification = sel.TraceSpecification();
            //editViewModel.SelectedWarehouse = editViewModel.AllWarehouse.FirstOrDefault();

            //var inven = editViewModel.Inventory;
            //datagridViewModel.AddNewItem(inven);

            //Assert.AreEqual(cnt, datagridViewModel.Items.Count);
            //Assert.IsFalse(datagridViewModel.Items.Any(x => x.Inven.UUID == sel.UUID));
        }

        [Ignore]
        [TestMethod]
        public void ReplaceTest2()
        {
            //new DummyDbData().Create();
            //var datagridViewModel = new InventoryDataGridViewModel();
            //var pip = datagridViewModel.SelectedItem;
            //var sel = datagridViewModel.SelectedItem.Inven;
            //var cnt = datagridViewModel.Items.Count;
            //var chn = datagridViewModel.Items.LastOrDefault().Inven;
            //var editViewModel = new InventoryEditorViewModel(chn);
            //editViewModel.ItemCount = 11;
            //editViewModel.SelectedItem = sel.TraceItem();
            //editViewModel.SelectedSpecification = sel.TraceSpecification();
            //editViewModel.SelectedWarehouse = editViewModel.AllWarehouse.FirstOrDefault();

            //var inven = editViewModel.Inventory;
            //datagridViewModel.ReplaceItem(inven);

            //Assert.AreEqual(cnt - 1, datagridViewModel.Items.Count);
        }

        [Ignore]
        [TestMethod]
        public void NonessentialTest()
        {
            new DummyDbData().Create();
            var dgViewModel = new InventoryDataGridViewModel();
            

            var copyItems = new ObservableCollection<InventoryPipe>(dgViewModel.Items);

            foreach (var i in copyItems)
            {
                Console.WriteLine("---");
                Console.WriteLine("{0}-{1}", i.Code, i.SubCode);
                var editViewModel = new InventoryEditorViewModel(i.Inven);
               // Console.WriteLine("{0}-{1}", editViewModel.Inventory.ItemUUID.Substring(0, 6).ToUpper(), editViewModel.Inventory.SpecificationUUID.Substring(0, 6).ToUpper());
                dgViewModel.AddNewItem(editViewModel.Inventory);
            }
        }
    }
}

