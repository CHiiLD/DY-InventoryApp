using System.Linq;

namespace R54IN0
{
    /// <summary>
    /// 재고 현황 CurrentStockWrapping
    /// </summary>
    public class InventoryWrapper : ProductWrapper<Inventory>
    {
        private Observable<Warehouse> _warehouse;

        public InventoryWrapper() : base()
        {
        }

        public InventoryWrapper(Inventory inventory)
            : base(inventory)
        {
        }

        public override Observable<Warehouse> Warehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                Product.WarehouseID = (_warehouse != null ? _warehouse.ID : null);
                Product.Save<Inventory>();
                OnPropertyChanged("Warehouse");
            }
        }

        public string Code
        {
            get
            {
                return Product.ItemID != null ? Product.ItemID.Substring(0, 6).ToUpper() : "";
            }
        }

        public string SubCode
        {
            get
            {
                return Product.SpecificationID != null ? Product.SpecificationID.Substring(0, 6).ToUpper() : "";
            }
        }

        public override string Remark
        {
            get
            {
                return Specification.Remark;
            }
        }

        protected override void SetProperies(Inventory record)
        {
            base.SetProperies(record);
            var fwd = FieldWrapperDirector.GetInstance();
            _warehouse = fwd.CreateCollection<Warehouse, Observable<Warehouse>>().
                Where(x => x.ID == record.WarehouseID).SingleOrDefault();
        }
    }
}