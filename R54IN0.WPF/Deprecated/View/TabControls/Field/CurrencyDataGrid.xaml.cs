using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class CurrencyDataGrid : UserControl
    {
        private FieldWrapperViewModel<Currency, Observable<Currency>> _viewModel;

        public CurrencyDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Currency, Observable<Currency>>(subject);
            DataContext = _viewModel;
        }
    }
}