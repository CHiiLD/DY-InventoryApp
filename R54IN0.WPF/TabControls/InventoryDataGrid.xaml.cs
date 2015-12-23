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
    public partial class InventoryDataGrid : UserControl
    {
        InventoryWrapperViewModel _viewModel;

        public InventoryWrapperViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public InventoryDataGrid()
        {
            InitializeComponent();
            ViewModelObserverSubject subject = ViewModelObserverSubject.GetInstance();
            _viewModel = new InventoryWrapperViewModel(subject);
            DataContext = _viewModel;
        }

        void OpenEditor(InventoryWrapperEditorViewModel helper)
        {
            InventoryEditorWindow irw = new InventoryEditorWindow();
            irw.Editor = helper;
            irw.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            irw.ShowDialog();
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(_viewModel);
            OpenEditor(helper);
        }

        void EditButton_Click(object sender, RoutedEventArgs e)
        {
            InventoryWrapperEditorViewModel helper = new InventoryWrapperEditorViewModel(_viewModel, _viewModel.SelectedItem);
            OpenEditor(helper);
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}