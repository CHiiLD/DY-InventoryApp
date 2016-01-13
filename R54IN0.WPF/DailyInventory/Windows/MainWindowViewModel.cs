using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class MainWindowViewModel : INotifyPropertyChanged, ICollectionViewModel<TabItem>
    {
        private ObservableCollection<TabItem> _items;
        private TabItem _selectedItem;

        public MainWindowViewModel()
        {
            InventoryStatus inventoryStatus = new InventoryStatus();
            IOStockStatus ioStockStatus = new IOStockStatus();
            _items = new ObservableCollection<TabItem>();
            _items.Add(new TabItem() { Content = inventoryStatus, Header = "재고 현황" });
            _items.Add(new TabItem() { Content = ioStockStatus, Header = "입출고 현황" });

            InventoryViewModel = inventoryStatus.DataContext as InventoryStatusViewModel;
            IOStockViewModel = ioStockStatus.DataContext as IOStockStatusViewModel;

            AppExitCommand = new RelayCommand<object>(ExecuteAppExitCommand);
            AboutAppCommand = new RelayCommand<object>(ExecuteAboutAppCommand);
            InventoryStatusCommand = new RelayCommand<object>(ExecuteInventoryStatusCommand);
            IOStockStatusProductModeCommand = new RelayCommand<object>(ExecuteIOStockStatusProductModeCommand);
            IOStockStatusDateModeCommand = new RelayCommand<object>(ExecuteIOStockStatusDateModeCommand);
            IOStockStatusProjectModeCommand = new RelayCommand<object>(ExecuteIOStockStatusProjectModeCommand);
        }

        private event PropertyChangedEventHandler _propertyChanged;

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

        public ICommand AboutAppCommand { get; set; }
        public ICommand AppExitCommand { get; set; }
        public ICommand InventoryStatusCommand { get; set; }
        public ICommand IOStockStatusProductModeCommand { get; set; }
        public ICommand IOStockStatusDateModeCommand { get; set; }
        public ICommand IOStockStatusProjectModeCommand { get; set; }

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

        private void ExecuteIOStockStatusProjectModeCommand(object obj)
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PROJECT;
        }

        private void ExecuteIOStockStatusDateModeCommand(object obj)
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_DATE;
        }

        private void ExecuteIOStockStatusProductModeCommand(object obj)
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == IOStockViewModel).Single();
            IOStockViewModel.SelectedGroupItem = IOStockStatusViewModel.GROUPITEM_PRODUCT;
        }

        private void ExecuteInventoryStatusCommand(object obj)
        {
            SelectedItem = Items.Where(x => ((UserControl)x.Content).DataContext == InventoryViewModel).Single();
        }

        private async void ExecuteAboutAppCommand(object obj)
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

        private void ExecuteAppExitCommand(object obj)
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