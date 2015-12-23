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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace R54IN0.WPF
{
    public partial class Finder : UserControl
    {
        MultiSelectFinderViewModel _viewModel;

        public Finder()
        {
            InitializeComponent();
            _viewModel = new MultiSelectFinderViewModel(FinderTreeView);
            DataContext = _viewModel;
        }

        public MultiSelectFinderViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }
    }
}