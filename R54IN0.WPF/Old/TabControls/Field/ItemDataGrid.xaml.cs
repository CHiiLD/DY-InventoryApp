using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class ItemDataGrid : UserControl
    {
        private ItemWrapperViewModel _viewModel;

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