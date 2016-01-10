using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class MakerDataGrid : UserControl
    {
        private FieldWrapperViewModel<Maker, Observable<Maker>> _viewModel;

        public MakerDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Maker, Observable<Maker>>(subject);
            DataContext = _viewModel;
        }
    }
}