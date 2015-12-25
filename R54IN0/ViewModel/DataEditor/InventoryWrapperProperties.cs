﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryWrapperProperties : IInventoryViewModelProperties, INotifyPropertyChanged
    {
        ItemWrapper _item;
        SpecificationWrapper _specification;
        FieldWrapper<Warehouse> _warehouse;

        public InventoryWrapperProperties()
        {
            Initialize();
        }

        public InventoryWrapperProperties(IProductWrapper inventoryWrapper)
        {
            Initialize(inventoryWrapper);
        }

        protected virtual void Initialize(IProductWrapper product = null)
        {
            if (product == null)
            {
                Stock = new Inventory();
                Quantity = 1;
            }
            else
            {
                Stock = product.Product.Clone() as Inventory;

                var inventory = product as InventoryWrapper;
                Item = inventory.Item;
                Specification = inventory.Specification;
                Warehouse = inventory.Warehouse;
            }
        }

        public virtual IStock Stock
        {
            get; set;
        }

        public FieldWrapper<Currency> Currency
        {
            get
            {
                return Item != null ? Item.SelectedCurrency : null;
            }
        }

        public virtual ItemWrapper Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                Stock.ItemUUID = _item != null ? _item.UUID : null;
                OnPropertyChanged("Item");
                OnPropertyChanged("Maker");
                OnPropertyChanged("Measure");
                OnPropertyChanged("Currency");
            }
        }

        public virtual int Quantity
        {
            get
            {
                return Stock.Quantity;
            }
            set
            {
                Stock.Quantity = value;
                OnPropertyChanged("Quantity");
                OnPropertyChanged("PurchaseUnitPrice");
                OnPropertyChanged("SalesUnitPrice");
                OnPropertyChanged("TotalPurchasePrice");
                OnPropertyChanged("TotalSalesPrice");
            }
        }

        public FieldWrapper<Maker> Maker
        {
            get
            {
                return Item != null ? Item.SelectedMaker : null;
            }
        }

        public FieldWrapper<Measure> Measure
        {
            get
            {
                return Item != null ? Item.SelectedMeasure : null;
            }
        }

        public virtual SpecificationWrapper Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                Stock.SpecificationUUID = _specification != null ? _specification.UUID : null; 
                OnPropertyChanged("Specification");
                OnPropertyChanged("PurchaseUnitPrice");
                OnPropertyChanged("SalesUnitPrice");
                OnPropertyChanged("TotalPurchasePrice");
                OnPropertyChanged("TotalSalesPrice");
            }
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return Specification != null ? Specification.PurchaseUnitPrice : 0;
            }
        }

        public decimal SalesUnitPrice
        {
            get
            {
                return Specification != null ? Specification.SalesUnitPrice : 0;
            }
        }

        public decimal PurchasePriceAmount
        {
            get
            {
                return Specification != null ? Specification.PurchaseUnitPrice * Quantity : 0;
            }
        }

        public decimal SalesPriceAmount
        {
            get
            {
                return Specification != null ? Specification.SalesUnitPrice * Quantity : 0;
            }
        }

        public virtual FieldWrapper<Warehouse> Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                if(Stock is Inventory)
                {
                    var inven = Stock as Inventory;
                    inven.WarehouseUUID = _warehouse != null ? _warehouse.UUID : null;
                }
                OnPropertyChanged("Warehouse");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
