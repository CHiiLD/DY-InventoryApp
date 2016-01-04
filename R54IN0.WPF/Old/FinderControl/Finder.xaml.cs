using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class Finder : UserControl
    {
        private ItemFinderViewModel _viewModel;

        public Finder()
        {
            InitializeComponent();
        }

        public ItemFinderViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                DataContext = _viewModel;
            }
        }
    }
}