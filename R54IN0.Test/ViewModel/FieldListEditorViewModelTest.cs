using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace R54IN0.Test
{
    [TestClass]
    public class FieldEditorViewModelTest
    {
        [TestMethod]
        public void CanCreateFieldEditorViewModelTest()
        {
            new FieldEditorViewModel<Currency>();
            new FieldEditorViewModel<Measure>();
            new FieldEditorViewModel<Warehouse>();
            new FieldEditorViewModel<Employee>();
        }

        [TestMethod]
        public void ItemsLoadTest()
        {
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();

            Assert.IsTrue(viewModel.Items.Count != 0);
        }

        [TestMethod]
        public void ItemSelectedWhenItemsLoad()
        {
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();

            Assert.IsTrue(viewModel.SelectedItem != null);
        }

        [TestMethod]
        public void SavePropertyNameTest()
        {
            string name = "TEST";
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();

            viewModel.SelectedItem.Name = name;
            string uuid = viewModel.SelectedItem.Field.UUID;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var measure = db.LoadByKey<Measure>(uuid);
                Assert.AreEqual(name, measure.Name);
            }
        }

        [TestMethod]
        public void SavePropertyIsDeletedTest()
        {
            bool hide = true;
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();

            string uuid = viewModel.SelectedItem.Field.UUID;
            viewModel.SelectedItem.IsDeleted = hide;

            using (var db = DatabaseDirector.GetDbInstance())
            {
                var measure = db.LoadByKey<Measure>(uuid);
                Assert.AreEqual(hide, measure.IsDeleted);
            }
        }

        [TestMethod]
        public void RemoveSelectedItemTest()
        {
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();
            var selectedItem = viewModel.SelectedItem;
            
            Assert.IsNotNull(selectedItem);

            viewModel.RemoveSelectedItem(null);

            if (viewModel.SelectedItem != null)
                Assert.AreNotEqual(selectedItem.Field.UUID, viewModel.SelectedItem.Field.UUID);
        }

        [TestMethod]
        public void AddNewItemTest()
        {
            new DummyDbData().Create();
            var viewModel = new FieldEditorViewModel<Measure>();
            var count = viewModel.Items.Count;
            var selectedItem = viewModel.SelectedItem;

            viewModel.AddNewItem(null);

            Assert.AreNotEqual(count, viewModel.Items.Count);
            Assert.AreEqual(count + 1, viewModel.Items.Count);
            Assert.AreNotEqual(selectedItem.Field.UUID, viewModel.SelectedItem.Field.UUID);
        }
    }

    internal class TddHelperTest
    {
        public TddHelperTest()
        {
        }

        internal void NewMethod(int v)
        {
            throw new NotImplementedException();
        }
    }
}