using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class MeasureDataGrid : UserControl
    {
        private FieldWrapperViewModel<Measure, Observable<Measure>> _viewModel;

        public MeasureDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Measure, Observable<Measure>>(subject);
            DataContext = _viewModel;
        }
    }
}