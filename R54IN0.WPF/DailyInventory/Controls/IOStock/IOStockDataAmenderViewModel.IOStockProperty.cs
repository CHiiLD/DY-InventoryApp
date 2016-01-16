using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
        public IOStockFormat Format
        {
            get
            {
                return _fmt;
            }
            set
            {
                _fmt = value;
                InitializeProperties(_fmt);
                NotifyPropertyChanged("");
            }
        }

        public IOStockType StockType
        {
            get
            {
                return _fmt.StockType;
            }
            set
            {
                _fmt.StockType = value;
                NotifyPropertyChanged("StockType");
                var ofd = ObservableFieldDirector.GetInstance();
                switch (value)
                {
                    case IOStockType.INCOMING:
                        Project = null;
                        ProjectText = null;
                        ClientList = ofd.CreateList<Supplier>();
                        IsEditableSpecification = true;
                        IsReadOnlyProductTextBox = false;
                        IsEnabledWarehouseComboBox = true;
                        IsEnabledProjectComboBox = false;
                        break;

                    case IOStockType.OUTGOING:
                        Warehouse = null;
                        WarehouseText = null;
                        ClientList = ofd.CreateList<Customer>();
                        IsEditableSpecification = false;
                        IsReadOnlyProductTextBox = true;
                        IsEnabledWarehouseComboBox = false;
                        IsEnabledProjectComboBox = true;
                        if (Product == null)
                            ProductText = null;
                        if (Inventory == null)
                        {
                            SpecificationText = null;
                            SpecificationMemo = null;
                            MakerText = null;
                            MeasureText = null;
                            Maker = null;
                            Measure = null;
                        }
                        break;
                }
                UpdateQuantityProperties();
            }
        }

        /// <summary>
        /// 기록된 날짜
        /// </summary>
        public virtual DateTime Date
        {
            get
            {
                return _fmt.Date;
            }
            set
            {
                _fmt.Date = value;
                NotifyPropertyChanged("Date");
            }
        }

        /// <summary>
        /// 제품의 개별적 입고가, 출고가
        /// </summary>
        public virtual decimal UnitPrice
        {
            get
            {
                return _fmt.UnitPrice;
            }
            set
            {
                _fmt.UnitPrice = value;
                NotifyPropertyChanged("UnitPrice");
                NotifyPropertyChanged("Amount");
            }
        }

        /// <summary>
        /// 입고 또는 출고 수량
        /// </summary>
        public virtual int Quantity
        {
            get
            {
                return _fmt.Quantity;
            }
            set
            {
                _fmt.Quantity = value;
                UpdateQuantityProperties();

                NotifyPropertyChanged("Quantity");
                NotifyPropertyChanged("Amount");
            }
        }

        /// <summary>
        /// 비고
        /// </summary>
        public string Memo
        {
            get
            {
                return _fmt.Memo;
            }
            set
            {
                _fmt.Memo = value;
                NotifyPropertyChanged("Memo");
            }
        }

        /// <summary>
        /// 거래처
        /// </summary>
        public Observable<Customer> Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _fmt.CustomerID = value != null ? value.ID : null;
                _customer = value;
                NotifyPropertyChanged("Customer");
            }
        }

        public Observable<Supplier> Supplier
        {
            get
            {
                return _supplier;
            }
            set
            {
                _fmt.SupplierID = value != null ? value.ID : null;
                _supplier = value;
                NotifyPropertyChanged("Supplier");
            }
        }

        /// <summary>
        /// 프로젝트
        /// </summary>
        public Observable<Project> Project
        {
            get
            {
                return _project;
            }
            set
            {
                _fmt.ProjectID = value != null ? value.ID : null;
                _project = value;
                NotifyPropertyChanged("Project");
            }
        }

        public IObservableInventoryProperties Inventory
        {
            get
            {
                return _inventory;
            }
            set
            {
                _fmt.InventoryID = value != null ? value.ID : null;
                _inventory = value;
                NotifyPropertyChanged("Inventory");

                if (value == null)
                {
                    SpecificationText = null;
                    SpecificationMemo = null;
                    Maker = null;
                    Measure = null;
                    MakerText = null;
                    MeasureText = null;
                }
                NotifyPropertyChanged("Maker");
                NotifyPropertyChanged("Measure");
                NotifyPropertyChanged("SpecificationMemo");
                if (RecordCommand != null)
                    RecordCommand.RaiseCanExecuteChanged();
            }
        }

        public string ID
        {
            get
            {
                return _fmt.ID;
            }
            set
            {
                _fmt.ID = value;
            }
        }

        public Observable<Employee> Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                _fmt.EmployeeID = value != null ? value.ID : null;
                _employee = value;
                NotifyPropertyChanged("Employee");
            }
        }

        public Observable<Warehouse> Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _fmt.WarehouseID = value != null ? value.ID : null;
                _warehouse = value;
                NotifyPropertyChanged("Warehouse");
            }
        }

        public int RemainingQuantity
        {
            get
            {
                return _fmt.RemainingQuantity;
            }
            set
            {
                _fmt.RemainingQuantity = value;
                NotifyPropertyChanged("RemainingQuantity");
            }
        }
    }
}