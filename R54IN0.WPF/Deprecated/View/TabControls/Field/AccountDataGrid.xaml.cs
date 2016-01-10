using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class ClientDataGrid : UserControl
    {
        private FieldWrapperViewModel<Client, ClientWrapper> _viewModel;

        public ClientDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Client, ClientWrapper>(subject);
            DataContext = _viewModel;
        }
    }
}