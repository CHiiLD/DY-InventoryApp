using System.Windows.Controls;

namespace R54IN0.WPF
{
    public partial class EmployeeDataGrid : UserControl
    {
        private FieldWrapperViewModel<Employee, Observable<Employee>> _viewModel;

        public EmployeeDataGrid()
        {
            InitializeComponent();
            CollectionViewModelObserverSubject subject = CollectionViewModelObserverSubject.GetInstance();
            _viewModel = new FieldWrapperViewModel<Employee, Observable<Employee>>(subject);
            DataContext = _viewModel;
        }
    }
}