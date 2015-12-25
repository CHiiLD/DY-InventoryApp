using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryWrapperProperties : IInventoryEditorViewModelProperties, INotifyPropertyChanged
    {
        protected IStockWrapper recordWrapper;

        public InventoryWrapperProperties()
        {
            Inventory inventory = new Inventory();
            recordWrapper = new InventoryWrapper(inventory);
            Quantity = 1;
        }

        public InventoryWrapperProperties(IStockWrapper recordWrapper)
        {
            if (recordWrapper != null)
            {
                var clone = recordWrapper.Record.Clone() as Inventory;
                this.recordWrapper = new InventoryWrapper(clone);
            }
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
                return recordWrapper.Item;
            }
            set
            {
                recordWrapper.Item = value;
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
                return recordWrapper.Quantity;
            }
            set
            {
                recordWrapper.Quantity = value;
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
                return recordWrapper.Specification;
            }
            set
            {
                recordWrapper.Specification = value;
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

        public FieldWrapper<Warehouse> Warehouse
        {
            get
            {
                return recordWrapper.Warehouse;
            }
            set
            {
                recordWrapper.Warehouse = value;
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
