using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    public class IOStockDataGridViewModel : ICollectionViewModel<IOStockDataGridItem>, INotifyPropertyChanged
    {
        private event PropertyChangedEventHandler _propertyChanged;

        private ObservableCollection<IOStockDataGridItem> _items;
        private IOStockDataGridItem _selectedItem;
        private Visibility _specificationMemoColumnVisibility;
        private Visibility _makerColumnVisibility;
        private Visibility _secondStockTypeColumnVisibility;
        private Visibility _remainQtyColumnVisibility;
        private bool _isReadOnly;

        

        public IOStockDataGridViewModel()
        {
            Items = new ObservableCollection<IOStockDataGridItem>();
            PreviewTextInputEventCommand = new RelayCommand<object>(ExecuteInventoryStatusTurnningCommand);
            ChangeAsProductsReportCommand = new RelayCommand<object>(ExecuteChangeAsProductsReportCommand);
            NewIOStockFormatAddCommand = new RelayCommand<object>(ExecuteNewIOStockFormatAddCommand);
            IOStockFormatModificationCommand = new RelayCommand<object>(ExecuteIOStockFormatModificationCommand);
            IOStockFormatDeletionCommand = new RelayCommand<object>(ExecuteIOStockFormatDeletionCommand);
            ChekcedIOStockFormatsDeletionCommand = new RelayCommand<object>(ExecuteChekcedIOStockFormatsDeletionCommand);
            CheckedIOStockFormatsCopyCommand = new RelayCommand<object>(ExecuteCheckedIOStockFormatsCopyCommand);
            InventoryStatusTurnningCommand = new RelayCommand<object>(ExecuteInventoryStatusTurnningCommand);

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

        public ObservableCollection<IOStockDataGridItem> Items
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

        public IOStockDataGridItem SelectedItem
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

        public Visibility SpecificationMemoColumnVisibility
        {
            get
            {
                return _specificationMemoColumnVisibility;
            }
            set
            {
                _specificationMemoColumnVisibility = value;
                NotifyPropertyChanged("SpecificationMemoColumnVisibility");
            }
        }

        public Visibility MakerColumnVisibility
        {
            get
            {
                return _makerColumnVisibility;
            }
            set
            {
                _makerColumnVisibility = value;
                NotifyPropertyChanged("MakerColumnVisibility");
            }
        }

        public Visibility SecondStockTypeColumnVisibility
        {
            get
            {
                return _secondStockTypeColumnVisibility;
            }
            set
            {
                _secondStockTypeColumnVisibility = value;
                NotifyPropertyChanged("SecondStockTypeColumnVisibility");
            }
        }

        public Visibility RemainQtyColumnVisibility
        {
            get
            {
                return _remainQtyColumnVisibility;
            }
            set
            {
                _remainQtyColumnVisibility = value;
                NotifyPropertyChanged("RemainQtyColumnVisibility");
            }
        }

        /// <summary>
        /// 수량을 제외한 모든열에 대한 IsReadOnly 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
            set
            {
                _isReadOnly = value;
                NotifyPropertyChanged("IsReadOnly");
            }
        }

        public ICommand PreviewTextInputEventCommand { get; set; }

        public ICommand ChangeAsProductsReportCommand { get; set; }

        public ICommand NewIOStockFormatAddCommand { get; set; }

        public ICommand IOStockFormatModificationCommand { get; set; }

        public ICommand IOStockFormatDeletionCommand { get; set; }

        public ICommand ChekcedIOStockFormatsDeletionCommand { get; set; }

        public ICommand CheckedIOStockFormatsCopyCommand { get; set; }

        public ICommand InventoryStatusTurnningCommand { get; set; }

        private void ExecuteChangeAsProductsReportCommand(object obj)
        {
            var id = SelectedItem.Inventory.Product.ID;
            if (id != null)
            {
                var node = TreeViewNodeDirector.GetInstance().SearchProductNode(id);
                if (node != null)
                {
                    MainWindowViewModel main = Application.Current.MainWindow.DataContext as MainWindowViewModel;
                    main.IOStockStatusProductModeCommand.Execute(null);
                    main.IOStockViewModel.TreeViewViewModel.SelectedNodes.Clear();
                    main.IOStockViewModel.TreeViewViewModel.SelectedNodes.Add(node);
                    main.IOStockViewModel.TreeViewViewModel.NotifyPropertyChanged("SelectedNodes");
                }
            }
        }

        private void ExecuteNewIOStockFormatAddCommand(object obj)
        {
            MainWindowViewModel main = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            main.IOStockViewModel.OpenAmender(SelectedItem.Inventory);
        }

        private void ExecuteIOStockFormatModificationCommand(object obj)
        {
            MainWindowViewModel main = Application.Current.MainWindow.DataContext as MainWindowViewModel;
            main.IOStockViewModel.OpenAmender(SelectedItem);
        }

        private void ExecuteIOStockFormatDeletionCommand(object obj)
        {
            
        }

        private void ExecuteChekcedIOStockFormatsDeletionCommand(object obj)
        {
            
        }

        private void ExecuteCheckedIOStockFormatsCopyCommand(object obj)
        {
            List<IOStockDataGridItem> list = new List<IOStockDataGridItem>();
            foreach (var item in Items)
            {
                //TODO 이제 추가된 새로운 데이터들의 수량만큼 재고수량과 잔여수량을 적용시켜야함 그리고 삭제 기능들을 넣자.. 
                if (item.IsChecked == true)
                {
                    IOStockDataGridItem copy = new IOStockDataGridItem(item);
                    copy.IsChecked = false;
                    copy.Date = DateTime.Now;
                    list.Add(copy);
                }
            }
            foreach(var item in list)
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(item);
        }

        private void ExecuteInventoryStatusTurnningCommand(object parameter)
        {
            var eventArgs = parameter as TextCompositionEventArgs;
            OnPreviewTextInputted(eventArgs.Source, eventArgs);
        }

        public void OnPreviewTextInputted(object sender, TextCompositionEventArgs e)
        {
            var datagrid = sender as DataGrid;
            if (datagrid != null)
            {
                IOStockDataGridItem item = datagrid.CurrentItem as IOStockDataGridItem;
                DataGridColumn column = datagrid.CurrentColumn;
                if (column.SortMemberPath.Contains("Name"))
                {
                    string propertyPath = column.SortMemberPath.Replace(".Name", "");
                    string[] paths = propertyPath.Split('.');
                    object property = item;
                    foreach (var path in paths)
                    {
                        property = property.GetType().GetProperty(path).GetValue(property, null);
                    }
                    if (property == null)
                        e.Handled = true;
                }
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}