using System;
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
        Observable<Warehouse> _warehouse;

        public InventoryWrapperProperties()
        {
            Initialize();
        }

        public InventoryWrapperProperties(IProductWrapper inventoryWrapper)
        {
            Initialize(inventoryWrapper);
        }

        /// <summary>
        /// 객체를 생성할 때 생성자에서 호출됩니다.
        /// </summary>
        /// <param name="product"></param>
        protected virtual void Initialize(IProductWrapper product = null)
        {
            if (product == null) //새로운 데이터를 추가할 경우
            {
                Stock = new Inventory();
                Quantity = 1;
            }
            else                //기존의 데이터를 수정할 경우
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

        public Observable<Currency> Currency
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
                Stock.ItemID = _item != null ? _item.ID : null;
                //ItemList에서 새로운 아이템을 선택할 경우 아래 프로퍼티를 새로고침한다.
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
                OnPropertyChanged("PurchasePriceAmount");
                OnPropertyChanged("SalesPriceAmount");
            }
        }

        public Observable<Maker> Maker
        {
            get
            {
                return Item != null ? Item.SelectedMaker : null;
            }
        }

        public Observable<Measure> Measure
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
                Stock.SpecificationID = _specification != null ? _specification.ID : null; 
                OnPropertyChanged("Specification");
                OnPropertyChanged("PurchaseUnitPrice");
                OnPropertyChanged("SalesUnitPrice");
                OnPropertyChanged("PurchasePriceAmount");
                OnPropertyChanged("SalesPriceAmount");
                OnPropertyChanged("InventoryQuantity");
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

        public virtual Observable<Warehouse> Warehouse
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
                    inven.WarehouseID = _warehouse != null ? _warehouse.ID : null;
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
