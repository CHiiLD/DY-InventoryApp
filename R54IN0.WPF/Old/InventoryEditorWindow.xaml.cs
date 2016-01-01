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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;

namespace R54IN0.WPF
{
    /// <summary>
    /// InventoryRegisterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InventoryEditorWindow : MetroWindow
    {
        InventoryWrapperEditorViewModel _viewModel;

        public InventoryWrapperEditorViewModel Editor
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

        public InventoryEditorWindow()
        {
            InitializeComponent();
        }

        async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            bool hasException = false;
            string message = null;
            try
            {
                _viewModel.Update();
                Close();
            }
            catch (Exception exception)
            {
                hasException = true;
                message = exception.Message;
            }
            if (hasException)
                await this.ShowMessageAsync("새로운 재고 데이터를 추가할 수 없습니다", message);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
