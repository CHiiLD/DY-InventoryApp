using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.Test
{
    public class DataGridSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }
    }

    [TestClass]
    public class INofityPropertiesChangedTest
    {
        [TestMethod]
        public void CanCreateDataGrid()
        {
            DataGrid db = new DataGrid();
        }

        [TestMethod]
        public void BindingTest()
        {
            DataGrid db = new DataGrid();
            ObservableCollection<DataGridSource> items = new ObservableCollection<DataGridSource>();
            var d = new DataGridSource() { Name = "A" };
            items.Add(d);
            db.ItemsSource = items;
            db.SelectedItem = items.FirstOrDefault();

            object value = db.SelectedValue;
            Assert.AreNotEqual(null, value);
            Assert.AreEqual(items.FirstOrDefault(), value);
            items.Remove(d);
            Assert.AreEqual(null, db.SelectedItem);
        }
    }
}