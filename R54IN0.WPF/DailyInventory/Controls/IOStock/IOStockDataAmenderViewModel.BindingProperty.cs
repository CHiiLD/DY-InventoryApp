﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        /// <summary>
        /// 제품 탐색기 뷰모델
        /// </summary>
        public MultiSelectTreeViewModelView TreeViewViewModel { get; set; }

        /// <summary>
        /// 제품 탐색기 열기 버튼의 명령어 
        /// </summary>
        public RelayCommand ProductSearchCommand { get; set; }

        /// <summary>
        /// 저장 버튼 명령어
        /// </summary>
        public RelayCommand RecordCommand { get; set; }

        /// <summary>
        /// 제품 탐색기에서 제품을 선택한 뒤, 확인 버튼의 Command 객체
        /// </summary>
        public RelayCommand ProductSelectCommand { get; set; }

        public bool IsEditableSpecification
        {
            get
            {
                return _isEditableSpecification;
            }
            set
            {
                _isEditableSpecification = value;
                NotifyPropertyChanged("IsEditableSpecification");
            }
        }

        /// <summary>
        /// 제품 탐색기 플라이아웃의 가시 여부 바인딩 프로퍼티
        /// </summary>
        public bool IsOpenFlyout
        {
            get
            {
                return _isOpenFlyout;
            }
            set
            {
                _isOpenFlyout = value;
                NotifyPropertyChanged("IsOpenFlyout");
            }
        }

        /// <summary>
        /// 단가의 합계 (입출된 개수 * 가격)
        /// </summary>
        public decimal Amount
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        /// <summary>
        /// 재고 수량
        /// </summary>
        public int InventoryQuantity
        {
            get
            {
                return _inventoryQuantity;
            }
            set
            {
                _inventoryQuantity = value;
                NotifyPropertyChanged("InventoryQuantity");
            }
        }

        #region ComboBox ItemsSource Property

        public IEnumerable<IObservableInventoryProperties> InventoryList
        {
            get
            {
                return _inventoryList;
            }
            set
            {
                _inventoryList = value;
                NotifyPropertyChanged("InventoryList");
            }
        }

        public IEnumerable<Observable<Maker>> MakerList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Maker>();
            }
        }

        public IEnumerable<Observable<Measure>> MeasureList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Measure>();
            }
        }

        public IEnumerable<IObservableField> ClientList
        {
            get
            {
                return _clientList;
            }
            set
            {
                _clientList = value;
                NotifyPropertyChanged("ClientList");
            }
        }

        public IEnumerable<Observable<Warehouse>> WarehouseList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Warehouse>();
            }
        }

        public IEnumerable<Observable<Employee>> EmployeeList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Employee>();
            }
        }

        public IEnumerable<Observable<Project>> ProjectList
        {
            get
            {
                var ofd = ObservableFieldDirector.GetInstance();
                return ofd.CreateList<Project>();
            }
        }

        #endregion ComboBox ItemsSource Property

        #region ComboBox SelectedItem Property

        /// <summary>
        /// Product를 변경할 시, ProductText, InventoryList 프로퍼티를 업데이트
        /// </summary>
        public Observable<Product> Product
        {
            get
            {
                return _product;
            }
            set
            {
                _product = value;
                if (_product != null)
                {
                    ProductText = _product.Name;
                    var searchList = ObservableInventoryDirector.GetInstance().SearchAsProductID(_product.ID);
                    InventoryList = searchList.Select(x => new NonSaveObservableInventory(new InventoryFormat(x.Format))).ToList();
                    if (InventoryList.Count() == 1)
                        Inventory = InventoryList.Single();
                }
                NotifyPropertyChanged("Product");
            }
        }

        public IObservableField Client
        {
            get
            {
                return StockType == IOStockType.INCOMING ? (IObservableField)Supplier : (IObservableField)Customer;
            }
            set
            {
                if (StockType == IOStockType.INCOMING)
                    Supplier = value as Observable<Supplier>;
                else if (StockType == IOStockType.OUTGOING)
                    Customer = value as Observable<Customer>;
                NotifyPropertyChanged("Client");
            }
        }

        public Observable<Maker> Maker
        {
            get
            {
                return Inventory != null ? Inventory.Maker : _maker;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Maker = value;
                else
                    _maker = value;
                NotifyPropertyChanged("Maker");
            }
        }

        public Observable<Measure> Measure
        {
            get
            {
                return Inventory != null ? Inventory.Measure : _measure;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Measure = value;
                else
                    _measure = value;
                NotifyPropertyChanged("Measure");
            }
        }

        #endregion ComboBox SelectedItem Property

        #region TextBox Property

        /// <summary>
        /// ProductText를 변경할 시, Product, Inventory, InventoryList 속성을 변경
        /// </summary>
        public string ProductText
        {
            get
            {
                return _productText;
            }
            set
            {
                _productText = value;
                if (_productText != null)
                {
                    //제품을 선택하였지만, 이후 다른 제품명을 입력한 경우 저장된 제품 객체를 null로 대입
                    if (Product != null && Product.Name != _productText)
                        Product = null;
                    if (Product == null && Inventory != null)
                        Inventory = null;
                    if (InventoryList != null)
                        InventoryList = null;
                }
                NotifyPropertyChanged("ProductText");
                if (RecordCommand != null)
                    RecordCommand.RaiseCanExecuteChanged();
            }
        }

        public string SpecificationText
        {
            get
            {
                return _specificationText;
            }
            set
            {
                _specificationText = value;
                NotifyPropertyChanged("SpecificationText");
                RecordCommand.RaiseCanExecuteChanged();
            }
        }

        public string SpecificationMemo
        {
            get
            {
                return Inventory != null ? Inventory.Memo : _specificationMemo;
            }
            set
            {
                if (Inventory != null)
                    Inventory.Memo = value;
                else
                    _specificationMemo = value;
                NotifyPropertyChanged("SpecificationMemo");
            }
        }

        public string MakerText
        {
            get
            {
                return _makerText;
            }
            set
            {
                _makerText = value;
                NotifyPropertyChanged("MakerText");
            }
        }

        public string MeasureText
        {
            get
            {
                return _measureText;
            }
            set
            {
                _measureText = value;
                NotifyPropertyChanged("MeasureText");
            }
        }

        public string ClientText
        {
            get
            {
                return _clientText;
            }
            set
            {
                _clientText = value;
                NotifyPropertyChanged("ClientText");
            }
        }

        public string WarehouseText
        {
            get
            {
                return _warehouseText;
            }
            set
            {
                _warehouseText = value;
                NotifyPropertyChanged("WarehouseText");
            }
        }

        public string ProjectText
        {
            get
            {
                return _projectText;
            }
            set
            {
                _projectText = value;
                NotifyPropertyChanged("ProjectText");
            }
        }

        public string EmployeeText
        {
            get
            {
                return _employeeText;
            }
            set
            {
                _employeeText = value;
                NotifyPropertyChanged("EmployeeText");
            }
        }

        #endregion TextBox Property

        #region IsEnabled Property

        public bool IsEnabledDatePicker
        {
            get
            {
                return _isEnabledDatePicker;
            }
            set
            {
                _isEnabledDatePicker = value;
                NotifyPropertyChanged("IsEnabledDatePicker");
            }
        }

        public bool IsEnabledWarehouseComboBox
        {
            get
            {
                return _isEnabledWarehouseComboBox;
            }
            set
            {
                _isEnabledWarehouseComboBox = value;
                NotifyPropertyChanged("IsEnabledWarehouseComboBox");
            }
        }

        public bool IsEnabledProjectComboBox
        {
            get
            {
                return _isEnabledProjectComboBox;
            }
            set
            {
                _isEnabledProjectComboBox = value;
                NotifyPropertyChanged("IsEnabledProjectComboBox");
            }
        }

        public bool IsEnabledInComingRadioButton
        {
            get
            {
                return _isEnabledInComingRadioButton;
            }
            set
            {
                _isEnabledInComingRadioButton = value;
                NotifyPropertyChanged("IsEnabledInComingRadioButton");
            }
        }

        public bool IsEnabledOutGoingRadioButton
        {
            get
            {
                return _isEnabledInOutGoingRadioButton;
            }
            set
            {
                _isEnabledInOutGoingRadioButton = value;
                NotifyPropertyChanged("IsEnabledOutGoingRadioButton");
            }
        }

        public bool IsEnabledSpecificationComboBox
        {
            get
            {
                return _isEnabledSpecificationComboBox;
            }
            set
            {
                _isEnabledSpecificationComboBox = value;
                NotifyPropertyChanged("IsEnabledSpecificationComboBox");
            }
        }

        public bool IsEnabledMeasureComboBox
        {
            get
            {
                return _isEnabledMeasureComboBox;
            }
            set
            {
                _isEnabledMeasureComboBox = value;
                NotifyPropertyChanged("IsEnabledMeasureComboBox");
            }
        }

        public bool IsEnabledMakerComboBox
        {
            get
            {
                return _isEnabledMakerComboBox;
            }
            set
            {
                _isEnabledMakerComboBox = value;
                NotifyPropertyChanged("IsEnabledMakerComboBox");
            }
        }

        #endregion IsEnabled Property

        #region IsReadOnly Property

        /// <summary>
        /// 제품 텍스트 박스의 텍스트 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnlyProductTextBox
        {
            get
            {
                return _isReadOnlyProductTextBox;
            }
            set
            {
                _isReadOnlyProductTextBox = value;
                NotifyPropertyChanged("IsReadOnlyProductTextBox");
            }
        }

        public bool IsReadOnlySpecificationMemoTextBox
        {
            get
            {
                return _isReadOnlySpecificationMemoTextBox;
            }
            set
            {
                _isReadOnlySpecificationMemoTextBox = value;
                NotifyPropertyChanged("IsReadOnlySpecificationMemoTextBox");
            }
        }

        #endregion IsReadOnly Property
    }
}