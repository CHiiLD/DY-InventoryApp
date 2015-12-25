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
    public partial class ItemDataGrid : UserControl
    {
        ItemWrapperViewModel _viewModel;

        public ItemWrapperViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public ItemDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new ItemWrapperViewModel(subject);
            DataContext = _viewModel;
        }
    }
}
