using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged, ICollectionViewModel<TabItem>
    {
        private static MainWindowViewModel _thiz;

        private ObservableCollection<TabItem> _items;
        private TabItem _selectedItem;

        private event PropertyChangedEventHandler _propertyChanged;

        public MainWindowViewModel()
        {
            InventoryStatus inventoryStatus = new InventoryStatus();
            IOStockStatus ioStockStatus = new IOStockStatus();
            _items = new ObservableCollection<TabItem>();
            _items.Add(new TabItem() { Content = inventoryStatus, Header = "재고 현황" });
            _items.Add(new TabItem() { Content = ioStockStatus, Header = "입출고 현황" });

            InventoryViewModel = inventoryStatus.DataContext as InventoryStatusViewModel;
            IOStockViewModel = ioStockStatus.DataContext as IOStockStatusViewModel;

            AppExitCommand = new RelayCommand(ExecuteAppExitCommand);
            AboutAppCommand = new RelayCommand(ExecuteAboutAppCommand);
            ChangeInventoryViewCommand = new RelayCommand(ExecuteSelectInventoryStatusViewCommand);
            ChangeIOStockViewByProductCommand = new RelayCommand(ExecuteChangeIOStockViewByProductCommand);
            ChangeIOStockByDateCommand = new RelayCommand(ExecuteChangeIOStockByDateCommand);
            ChangeIOStockByProjectCommand = new RelayCommand(ExecuteChangeIOStockByProjectCommand);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public string AppName
        {
            get
            {
                return "Daily Inventory";
            }
        }

        public string AppVersion
        {
            get
            {
                return "Version beta 0.101";
            }
        }

        public string Copyright
        {
            get
            {
                return "copyright ⓒ 2015 DongYang FA All Rights Reserved";
            }
        }

        public RelayCommand AboutAppCommand { get; set; }
        public RelayCommand AppExitCommand { get; set; }
        public RelayCommand ChangeInventoryViewCommand { get; set; }
        public RelayCommand ChangeIOStockViewByProductCommand { get; set; }
        public RelayCommand ChangeIOStockByDateCommand { get; set; }
        public RelayCommand ChangeIOStockByProjectCommand { get; set; }

        public ObservableCollection<TabItem> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public TabItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        public InventoryStatusViewModel InventoryViewModel { get; set; }

        public IOStockStatusViewModel IOStockViewModel { get; set; }

        public static MainWindowViewModel GetInstance()
        {
            if (_thiz == null)
                _thiz = new MainWindowViewModel();
            return _thiz;
        }

        /// <summary>
        /// 입출고 데이터 등록창을 띄운다.
        /// </summary>
        /// <param name="productID"></param>
        public void ShowIOStockDataAmenderWindow(string productID)
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var product = ofd.Search<Product>(productID);
            if (product != null)
                IOStockViewModel.OpenIOStockDataAmenderWindow(product);
        }

        /// <summary>
        /// 재고현황 탭아이템으로 이동한 후 제품 데이터를 데이터그리드에 출력한다.
        /// </summary>
        /// <param name="node"></param>
        public void ShowInventoryStatus(string productID)
        {
            var viewmodel = SelectedItem.Content as UserControl;
            if (viewmodel.DataContext != InventoryViewModel)
                ExecuteChangeIOStockViewByProductCommand();

            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchProductNode(productID);
            if (node != null)
            {
                MultiSelectTreeViewModelView treeView = InventoryViewModel.TreeViewViewModel;
                treeView.SelectedNodes.Clear();
                treeView.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
            }
        }

        /// <summary>
        /// 제품별 입출고 현황 탭아이템으로 이동 후 제품의 입출고 데이터를 데이터그리드에 출력한다.
        /// </summary>
        /// <param name="node"></param>
        public void ShowIOStockStatusByProduct(string productID)
        {
            var viewmodel = SelectedItem.Content as UserControl;
            if (viewmodel.DataContext != IOStockViewModel)
                ExecuteChangeIOStockViewByProductCommand();

            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchProductNode(productID);
            if (node != null)
            {
                MultiSelectTreeViewModelView treeView = IOStockViewModel.TreeViewViewModel;
                treeView.SelectedNodes.Clear();
                treeView.ExecuteNodesSelectedEventCommand(new SelectionChangedCancelEventArgs(new TreeViewNode[] { node }, null));
            }
        }

        private void ExecuteChangeIOStockByProjectCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PROJECT;
        }

        private void ExecuteChangeIOStockByDateCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_DATE;
        }

        private void ExecuteChangeIOStockViewByProductCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
        }

        private void ExecuteSelectInventoryStatusViewCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == InventoryViewModel).Single();
        }

        private async void ExecuteAboutAppCommand()
        {
            Window window = Application.Current.MainWindow;
            if (window != null && window is MetroWindow)
            {
                MetroWindow metroWindow = window as MetroWindow;
                await metroWindow.ShowMessageAsync(
                    AppName,
                    AppVersion + "\n\n\n\n\n\n\n" + Copyright,
                    MessageDialogStyle.Affirmative,
                    new MetroDialogSettings() { AffirmativeButtonText = "확인", ColorScheme = MetroDialogColorScheme.Accented });
            }
        }

        private void ExecuteAppExitCommand()
        {
            Application.Current.Shutdown(110);
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}