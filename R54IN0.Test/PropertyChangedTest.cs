using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace R54IN0.Test
{
    public class P : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        string _value;

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        public void SetValue(string v)
        {
            _value = v;
        }

        public void Nofity()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }
    }

    [Ignore]
    [TestClass]
    public class PropertyChangedTest
    {
        [TestMethod]
        public void INotifyPropertyChangedTest()
        {
            string text = "b";

            //DataGird 

            ObservableCollection<P> oc = new ObservableCollection<P>();
            oc.Add(new P() { Value = "a" });
            oc.First().SetValue(text);

            Assert.AreEqual(text, oc.First().Value);

            oc.First().Nofity();

            Assert.AreEqual(text, oc.First().Value);
        }
    }
}
