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
    /// <summary>
    /// FieldEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MakerFieldEditor : UserControl
    {
        FieldEditorViewModel<Maker> _viewModel;

        public MakerFieldEditor()
        {
            InitializeComponent();
            DataContext = _viewModel = new FieldEditorViewModel<Maker>();
        }

        private void ItemAddButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddNewItem();
        }

        private void ItemRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveSelectedItem();
        }
    }
}