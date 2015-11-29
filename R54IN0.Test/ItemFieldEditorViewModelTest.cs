using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class ItemFieldEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateItemFieldEditorViewModelTest()
        {
            var viewModel = new ItemFieldEditorViewModel();
        }

        [TestMethod]
        public void CheckViewModelLoadedItems()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();
            int viewmodelItemsCount = viewModel.Items.Count;
            int dbItemCount = DatabaseDirector.GetDbInstance().LoadAll<Item>().Length;
            Assert.AreEqual(dbItemCount, viewmodelItemsCount);
        }

        [TestMethod]
        public void AutoLoadSpecificationWhenSelectItem()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();
            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();
            viewModel.SelectedItem = viewModel.Items.First();
            if (viewModel.SelectedSpecification != null)
                Assert.AreEqual(viewModel.SelectedItem.Field.UUID, viewModel.Specifications.First().Field.ItemUUID);
        }

        [TestMethod]
        public void FindMeasureAutomatically()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();
            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();

            var measure = viewModel.Items.First().SelectedMeasure;
            Assert.AreEqual(items.First().MeasureUUID, measure.UUID);
        }

        [TestMethod]
        public void SpecificationAddRemoveTest()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();

            viewModel.AddNewItem();

            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();

            viewModel.RemoveSelectedSpecification();
            viewModel.RemoveSelectedSpecification();
            viewModel.RemoveSelectedSpecification();

            Assert.AreEqual(1, viewModel.Specifications.Count());
        }

        [TestMethod]
        public void ItemAddRemoveTest()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();
            var count = viewModel.Items.Count;
            viewModel.AddNewItem();
            Assert.AreEqual(count + 1, viewModel.Items.Count);
            viewModel.RemoveSelectedItem();
            Assert.AreEqual(count, viewModel.Items.Count);
        }

        [TestMethod]
        public void ItemsSaveTest()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();
            var count = viewModel.Items.Count;

            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel = new ItemFieldEditorViewModel();

            //Assert.AreEqual(count, viewModel.Items.Count);

            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel.AddNewItem();
            viewModel.Save();

            Assert.AreEqual(count + 8, viewModel.Items.Count);
        }

        [TestMethod]
        public void SpecificationsSaveTest()
        {
            new DummyDbData().Create();
            var viewModel = new ItemFieldEditorViewModel();

            var count = viewModel.Specifications.Count;

            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel = new ItemFieldEditorViewModel();

           // Assert.AreEqual(count, viewModel.Specifications.Count);

            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.Save();

            Assert.AreEqual(count + 8, viewModel.Specifications.Count);
        }
    }
}