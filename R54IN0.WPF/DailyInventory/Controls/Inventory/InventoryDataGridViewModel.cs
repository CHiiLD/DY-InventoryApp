﻿using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public class InventoryDataGridViewModel : ICollectionViewModel<ObservableInventory>, INotifyPropertyChanged
    {
        private ObservableCollection<ObservableInventory> _items;
        private ObservableInventory _selectedItem;
        private bool _isReadOnly;
        private Visibility _productColumnVisibility;
        private Visibility _makerColumnVisibility;
        private Visibility _measureColumnVisibility;

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

        /// <summary>
        /// 생성자
        /// </summary>
        public InventoryDataGridViewModel()
        {
            _items = new ObservableCollection<ObservableInventory>();
            SearchAsIOStockRecordCommand = new RelayCommand(ExecuteSearchAsIOStockRecordCommand, IsSelected);
            IOStockAmenderWindowCallCommand = new RelayCommand(ExecuteIOStockAmenderWindowCallCommand, IsSelected);
            InventoryDataDeletionCommand = new RelayCommand(ExecuteInventoryDataDeletionCommand, IsSelected);

            CellEditEndingEventCommand = new RelayCommand<DataGridCellEditEndingEventArgs>(ExecuteCellEditEndingEventCommand);
        }

        public RelayCommand<DataGridCellEditEndingEventArgs> CellEditEndingEventCommand { get; set; }

        #region ContextMenu MenuItem Binding Command

        /// <summary>
        /// 선택된 재고데이터를 입출고 데이터로 보기
        /// </summary>
        public RelayCommand SearchAsIOStockRecordCommand { get; set; }

        /// <summary>
        /// 선택된 재고데이터를 기반으로 새로운 입출고 데이터를 등록한다.
        /// </summary>
        public RelayCommand IOStockAmenderWindowCallCommand { get; set; }

        /// <summary>
        /// 선택된 재고데이터를 삭제한다.
        /// </summary>
        public RelayCommand InventoryDataDeletionCommand { get; set; }

        #endregion ContextMenu MenuItem Binding Command

        /// <summary>
        /// 제품열 보기/숨기기
        /// </summary>
        public Visibility ProductColumnVisibility
        {
            get
            {
                return _productColumnVisibility;
            }
            set
            {
                _productColumnVisibility = value;
                NotifyPropertyChanged("ProductColumnVisibility");
            }
        }

        /// <summary>
        /// 제조사열 보기/숨기기
        /// </summary>
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

        /// <summary>
        /// 단위열 보기/숨기기
        /// </summary>
        public Visibility MeasureColumnVisibility
        {
            get
            {
                return _measureColumnVisibility;
            }
            set
            {
                _measureColumnVisibility = value;
                NotifyPropertyChanged("MeasureColumnVisibility");
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

        public ObservableCollection<ObservableInventory> Items
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

        public ObservableInventory SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
                SearchAsIOStockRecordCommand.RaiseCanExecuteChanged();
                IOStockAmenderWindowCallCommand.RaiseCanExecuteChanged();
                InventoryDataDeletionCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsSelected()
        {
            return SelectedItem != null;
        }

        /// <summary>
        /// 선택된 재고 데이터를 삭제한다.
        /// </summary>
        private async void ExecuteInventoryDataDeletionCommand()
        {
            if (SelectedItem != null)
            {
                var item = SelectedItem;
                ObservableInventoryDirector.GetInstance().Remove(item);
                CollectionViewModelObserverSubject.GetInstance().NotifyItemDeleted(item);
                await DbAdapter.GetInstance().DeleteAsync(item.Format);
                SelectedItem = null;
            }
        }

        /// <summary>
        /// 입출고 데이터 수정창을 연다.
        /// </summary>
        private void ExecuteIOStockAmenderWindowCallCommand()
        {
            MainWindowViewModel.GetInstance().IOStockViewModel.OpenIOStockDataAmenderWindow(SelectedItem);
        }

        /// <summary>
        /// 선택된 재고 데이터를 기반으로 입출고 현황으로 탭 아이템을 변경한다.
        /// </summary>
        private void ExecuteSearchAsIOStockRecordCommand()
        {
            if (SelectedItem != null)
                MainWindowViewModel.GetInstance().ShowIOStockStatusByProduct(SelectedItem.Product.ID);
        }

        private void ExecuteCellEditEndingEventCommand(DataGridCellEditEndingEventArgs e)
        {
            DataGridColumn column = e.Column;
            DataGridRow row = e.Row;
            TextBox textBox = e.EditingElement as TextBox;
            string sortMemberPath = column.SortMemberPath;
            ObservableInventory item = row.Item as ObservableInventory;
            if (!sortMemberPath.Contains("Name") || item == null || textBox == null)
                return;
            string[] paths = column.SortMemberPath.Replace(".Name", "").Split('.');
            object property = item;
            foreach (var path in paths)
                property = property.GetType().GetProperty(path).GetValue(property, null);
            if (property == null)
            {
                string propertyName = paths.Last();
                switch (propertyName)
                {
                    case "Maker":
                        item.Maker = new Observable<Maker>() { Name = textBox.Text };
                        break;

                    case "Measure":
                        item.Measure = new Observable<Measure>() { Name = textBox.Text };
                        break;
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