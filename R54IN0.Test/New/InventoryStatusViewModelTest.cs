using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using R54IN0.WPF;
using System.Windows;

namespace R54IN0.Test.New
{
    [TestClass]
    public class InventoryStatusViewModelTest
    {
        [TestMethod]
        public void CanCreate()
        {
            new InventoryStatusViewModel();
        }

        [TestMethod]
        public void DataGridInitializationTest()
        {
            var viewmodel = new InventoryStatusViewModel();
            Assert.IsNotNull(viewmodel.DataGridViewModel1.Items);
            Assert.IsNotNull(viewmodel.DataGridViewModel2.Items);
            Assert.IsTrue(viewmodel.DataGridViewModel1.Items.Count >= viewmodel.DataGridViewModel2.Items.Count);
        }

        [TestMethod]
        public void ColumnCheckBoxTest()
        {
            var viewmodel = new InventoryStatusViewModel();

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureVisibility);

            viewmodel.ShowMakerColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = false;
            Assert.AreEqual(Visibility.Collapsed, viewmodel.DataGridViewModel1.MeasureVisibility);

            viewmodel.ShowMakerColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MakerVisibility);

            viewmodel.ShowProductColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.ProductVisibility);

            viewmodel.ShowMeasureColumn = true;
            Assert.AreEqual(Visibility.Visible, viewmodel.DataGridViewModel1.MeasureVisibility);

        }
    }
}