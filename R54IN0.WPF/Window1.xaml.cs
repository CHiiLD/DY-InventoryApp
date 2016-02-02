using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace R54IN0.WPF
{
    public class TestClass
    {
        public string Name { get; set; }
        public string[] Sources { get; set; }
    }

    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : Window
    {
        public TestClass[] Items
        {
            get; set;
        }

        public Window1()
        {
            Items = new TestClass[] { new TestClass() { Name = "first", Sources = new string[] { "1", "2" }}};
            InitializeComponent();
            DataContext = this;
        }
    }
}
