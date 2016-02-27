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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
            AppExitCommand = new RelayCommand(ExecuteAppExitCommand);
            AboutAppCommand = new RelayCommand(ExecuteAboutAppCommand);
            ChangeInventoryViewCommand = new RelayCommand(ExecuteSelectInventoryStatusViewCommand, CanExecuteMenuItemCommand);
            ChangeIOStockViewByProductCommand = new RelayCommand(ExecuteChangeIOStockViewByProductCommand, CanExecuteMenuItemCommand);
            ChangeIOStockByDateCommand = new RelayCommand(ExecuteChangeIOStockByDateCommand, CanExecuteMenuItemCommand);
            ChangeIOStockByProjectCommand = new RelayCommand(ExecuteChangeIOStockByProjectCommand, CanExecuteMenuItemCommand);
            OpenFieldManagerWindow = new RelayCommand(ExecuteOpenFieldManagerWindow, CanExecuteMenuItemCommand);

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

            Dispatcher.CurrentDispatcher.BeginInvoke(new Func<Task>(InitializeAsync));
        }

        private bool CanExecuteMenuItemCommand()
        {
            return InventoryViewModel != null && IOStockViewModel != null;
        }

        public async Task InitializeAsync()
        {
            //TODO MySQL 또는 서버에 접속할 수가 없다면 Dialog로 이를 알려주어야 한다. 
            //Dialog의 버튼은 다시 접속하기, 앱 종료하기 기능을 구현해준다.
            try
            {
                await DataDirector.InitialzeInstanceAsync();

                InventoryStatusControl inventoryStatus = new InventoryStatusControl();
                IOStockStatusControl ioStockStatus = new IOStockStatusControl();

                InventoryViewModel = inventoryStatus.DataContext as InventoryStatusViewModel;
                IOStockViewModel = ioStockStatus.DataContext as IOStockStatusViewModel;

                Items = new ObservableCollection<TabItem>();
                Items.Add(new TabItem() { Content = inventoryStatus, Header = "재고 현황" });
                Items.Add(new TabItem() { Content = ioStockStatus, Header = "입출고 현황" });

                ChangeInventoryViewCommand.RaiseCanExecuteChanged();
                ChangeIOStockViewByProductCommand.RaiseCanExecuteChanged();
                ChangeIOStockByDateCommand.RaiseCanExecuteChanged();
                ChangeIOStockByProjectCommand.RaiseCanExecuteChanged();
                OpenFieldManagerWindow.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {

            }
            finally
            {

            }
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

        public IEnumerable<AccentColorMenuData> AccentColors { get; set; }

        public IEnumerable<AppThemeMenuData> AppThemes { get; set; }

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

        public RelayCommand AboutAppCommand { get; private set; }
        public RelayCommand AppExitCommand { get; private set; }
        public RelayCommand ChangeInventoryViewCommand { get; private set; }
        public RelayCommand ChangeIOStockViewByProductCommand { get; private set; }
        public RelayCommand ChangeIOStockByDateCommand { get; private set; }
        public RelayCommand ChangeIOStockByProjectCommand { get; private set; }
        public RelayCommand OpenFieldManagerWindow { get; private set; }

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

        public InventoryStatusViewModel InventoryViewModel
        {
            get;
            set;
        }

        public IOStockStatusViewModel IOStockViewModel
        {
            get;
            set;
        }

        #endregion ViewModel

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
        public void OpenStockManagerAsProdID(string productID)
        {
            var ofd = DataDirector.GetInstance();
            var product = ofd.SearchField<Product>(productID);
            if (product != null)
                IOStockViewModel.OpenManager(product);
        }

        /// <summary>
        /// 입출고 데이터 등록창을 띄운다.
        /// </summary>
        /// <param name="inventoryID"></param>
        public void OpenStockManagerAsInvID(string inventoryID)
        {
            var ofd = DataDirector.GetInstance();
            var inventory = ofd.SearchInventory(inventoryID);
            if (inventory != null)
                IOStockViewModel.OpenManager(inventory);
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
            IOStockViewModel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PROJECT;
        }

        private void ExecuteChangeIOStockByDateCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_DATE;
        }

        private void ExecuteChangeIOStockViewByProductCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedDataGridGroupOption = IOStockStatusViewModel.DATAGRID_OPTION_PRODUCT;
        }

        private void ExecuteSelectInventoryStatusViewCommand()
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == InventoryViewModel).Single();
        }

        private void ExecuteAboutAppCommand()
        {
            Window main = Application.Current.MainWindow;
            if (main != null && main is MetroWindow)
            {
                MetroWindow metroWindow = main as MetroWindow;
                metroWindow.ShowMessageAsync(
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

        private void ExecuteOpenFieldManagerWindow()
        {
            Window main = Application.Current.MainWindow;
            if (main != null)
            {
                Window win = new FieldManagerWindow();
                win.Owner = main;
                win.ShowDialog();
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}