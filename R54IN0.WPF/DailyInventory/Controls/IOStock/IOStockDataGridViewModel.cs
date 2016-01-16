﻿using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public partial class IOStockDataGridViewModel : ICollectionViewModel<IOStockDataGridItem>, INotifyPropertyChanged
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
            PreviewTextInputEventCommand = new RelayCommand<TextCompositionEventArgs>(ExecutePreviewTextInputEventCommand);

            SearchAsIOStockRecordCommand = new RelayCommand(ExecuteSearchAsIOStockRecordCommand, IsSelected);
            NewIOStockFormatAdditionCommand = new RelayCommand(ExecuteNewIOStockFormatAdditionCommand);
            IOStockFormatModificationCommand = new RelayCommand(ExecuteIOStockFormatModificationCommand, IsSelected);
            IOStockFormatDeletionCommand = new RelayCommand(ExecuteIOStockFormatDeletionCommand, IsSelected);
            ChekcedIOStockFormatsDeletionCommand = new RelayCommand(ExecuteChekcedIOStockFormatsDeletionCommand);
            CheckedIOStockFormatsCopyCommand = new RelayCommand(ExecuteCheckedIOStockFormatsCopyCommand);
            SearchAsInventoryRecordCommand = new RelayCommand(ExecuteSearchAsInventoryRecordCommand, IsSelected);
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

        public RelayCommand<TextCompositionEventArgs> PreviewTextInputEventCommand { get; set; }

        private void ExecutePreviewTextInputEventCommand(TextCompositionEventArgs e)
        {
            var datagrid = e.Source as DataGrid;
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