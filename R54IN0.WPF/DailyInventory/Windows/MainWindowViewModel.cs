using GalaSoft.MvvmLight.Command;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            InventoryStatusControl inventoryStatus = new InventoryStatusControl();
            IOStockStatusControl ioStockStatus = new IOStockStatusControl();

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
            AddNewIOStockCommand = new RelayCommand(ExecuteAddNewIOStockCommand);

            AccentColors = ThemeManager.Accents.Select(a => new AccentColorMenuData()
            {
                Name = a.Name,
                ColorBrush = a.Resources["AccentColorBrush"] as Brush
            });

            AppThemes = ThemeManager.AppThemes.Select(a => new AppThemeMenuData()
            {
                Name = a.Name,
                BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                ColorBrush = a.Resources["WhiteColorBrush"] as Brush
            });
        }

        private void ExecuteAddNewIOStockCommand()
        {
            IOStockViewModel.OpenIOStockDataAmenderWindow();
        }

        public IEnumerable<AccentColorMenuData> AccentColors { get; set; }

        public IEnumerable<AppThemeMenuData> AppThemes { get; set; }

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
                return "DailY Inventory";
            }
        }

        public string AppVersion
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Version version = assembly.GetName().Version;
                return "Version beta " + version.ToString();
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
        public RelayCommand AddNewIOStockCommand { get; set; }

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

        #region ViewModel

        public InventoryStatusViewModel InventoryViewModel { get; set; }

        public IOStockStatusViewModel IOStockViewModel { get; set; }

        #endregion

        public object CurrentViewModel
        {
            get
            {
                var viewmodel = SelectedItem.Content as UserControl;
                return viewmodel.DataContext;
            }
        }

        public static MainWindowViewModel GetInstance()
        {
            if (_thiz == null)
                _thiz = new MainWindowViewModel();
            return _thiz;
        }

        public static void Destory()
        {
            _thiz = null;
        }

        /// <summary>
        /// 입출고 데이터 등록창을 띄운다.
        /// </summary>
        /// <param name="productID"></param>
        public void ShowAmenderWindowAsProductID(string productID)
        {
            var ofd = InventoryDataCommander.GetInstance();
            var product = ofd.SearchObservableField<Product>(productID);
            if (product != null)
                IOStockViewModel.OpenIOStockDataAmenderWindow(product);
        }
        
        /// <summary>
        /// 입출고 데이터 등록창을 띄운다.
        /// </summary>
        /// <param name="inventoryID"></param>
        public void ShowAmenderWindowAsInventoryID(string inventoryID)
        {
            var ofd = InventoryDataCommander.GetInstance();
            var inventory = ofd.SearchObservableInventory(inventoryID);
            if (inventory != null)
                IOStockViewModel.OpenIOStockDataAmenderWindow(inventory);
        }

        /// <summary>
        /// 재고현황 탭아이템으로 이동한 후 제품 데이터를 데이터그리드에 출력한다.
        /// </summary>
        /// <param name="node"></param>
        public void ShowInventoryStatus(string observableObjectID)
        {
            if (CurrentViewModel != InventoryViewModel)
                ExecuteSelectInventoryStatusViewCommand();

            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchObservableObjectNode(observableObjectID);
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
        public void ShowIOStockStatus(string observableObjectID)
        {
            if (CurrentViewModel != IOStockViewModel)
                ExecuteChangeIOStockViewByProductCommand();

            TreeViewNode node = TreeViewNodeDirector.GetInstance().SearchObservableObjectNode(observableObjectID);
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