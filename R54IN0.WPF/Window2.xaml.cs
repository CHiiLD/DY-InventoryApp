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
    public class Nami
    {
        public string Name
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Window2.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window2 : Window
    {
        public List<Nami> Items
        {
            get;
            set;
        }

        public Window2()
        {
            Items = new List<Nami>();
            Items.Add(new Nami() { Name = "A"});
            Items.Add(new Nami() { Name = "B" });
            InitializeComponent();
        }
    }
}
