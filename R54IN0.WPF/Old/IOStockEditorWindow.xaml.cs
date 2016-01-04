using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    /// <summary>
    /// IOStockRegisterWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IOStockEditorWindow : MetroWindow
    {
        private StockWrapperEditorViewModel _viewModel;

        public FinderViewModel Finder
        {
            get; set;
        }

        public StockWrapperEditorViewModel Editor
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
                Finder = _viewModel.CreateFinderViewModel(FinderTreeView);
                FinderTreeView.GetBindingExpression(TreeViewEx.ItemsSourceProperty).UpdateTarget();
                FinderTreeView.GetBindingExpression(TreeViewEx.SelectedItemsProperty).UpdateTarget();
                DataContext = _viewModel;
            }
        }

        public IOStockEditorWindow()
        {
            InitializeComponent();
        }

        private async void ItemAddButton_Click(object sender, RoutedEventArgs e)
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
                await this.ShowMessageAsync("새로운 입출고 데이터를 추가할 수 없습니다", message);
        }

        private void ItemRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}