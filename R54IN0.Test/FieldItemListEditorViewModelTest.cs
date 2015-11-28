using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace R54IN0.Test
{
    [TestClass]
    public class FieldItemListEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateFieldItemListEditorViewModel()
        {
            var viewModel = new FieldItemListEditorViewModel();
        }

        [TestMethod]
        public void CheckViewModelLoadedItems()
        {
            new DummyDbData().Create();
            var viewModel = new FieldItemListEditorViewModel();
            int viewmodelItemsCount = viewModel.Items.Count;
            int dbItemCount = DatabaseDirector.GetDbInstance().LoadAll<Item>().Length;
            Assert.AreEqual(dbItemCount, viewmodelItemsCount);
        }

        [TestMethod]
        public void AutoLoadSpecificationWhenSelectItem()
        {
            new DummyDbData().Create();
            var viewModel = new FieldItemListEditorViewModel();
            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();
            viewModel.SelectedItem = viewModel.Items.First();
            if (viewModel.SelectedItem != null)
                Assert.AreEqual(viewModel.SelectedItem.Item.UUID, viewModel.Specifications.First().Specification.ItemUUID);
        }

        [TestMethod]
        public void FindMeasureAutomatically()
        {
            new DummyDbData().Create();
            var viewModel = new FieldItemListEditorViewModel();
            var items = DatabaseDirector.GetDbInstance().LoadAll<Item>();

            var measure = viewModel.Items.First().SelectedMeasure;
            Assert.AreEqual(items.First().MeasureUUID, measure.UUID);
        }

        [TestMethod]
        public void SpecificationAddRemoveTest()
        {
            new DummyDbData().Create();
            var viewModel = new FieldItemListEditorViewModel();

            viewModel.AddNewItem();

            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();
            viewModel.AddNewSpecification();

            viewModel.RemoveSelectedSpecification();
            viewModel.RemoveSelectedSpecification();
            viewModel.RemoveSelectedSpecification();

            Assert.AreEqual(null, viewModel.SelectedSpecification);
            Assert.AreEqual(0, viewModel.Specifications.Count());
        }

        [TestMethod]
        public void ItemAddRemoveTest()
        {
            new DummyDbData().Create();
            var viewModel = new FieldItemListEditorViewModel();
            var count = viewModel.Items.Count;
            viewModel.AddNewItem();
            Assert.AreEqual(count + 1, viewModel.Items.Count);
            viewModel.RemoveSelectedItem();
            Assert.AreEqual(count, viewModel.Items.Count);
        }
    }
}
