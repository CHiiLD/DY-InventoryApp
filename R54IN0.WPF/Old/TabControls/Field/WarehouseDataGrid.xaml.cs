using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class WarehouseDataGrid : UserControl
    {
        private FieldWrapperViewModel<Warehouse, Observable<Warehouse>> _viewModel;

        public WarehouseDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Warehouse, Observable<Warehouse>>(subject);
            DataContext = _viewModel;
        }
    }
}