using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class InventoryEditorViewModel : INotifyPropertyChanged
    {
        IEnumerable<Specification> _allSpecification;
        Inventory _inventory;
        Item _item;
        Warehouse _warehouse;
        Specification _specification;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsEditMode { get; private set; }

        public Inventory Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public IEnumerable<Item> AllItem
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Item>();
                }
            }
        }

        public IEnumerable<Specification> AllSpecification
        {
            get
            {
                return _allSpecification;
            }
            set
            {
                _allSpecification = value;
                OnPropertyChanged("AllSpecification");
            }
        }

        public IEnumerable<Warehouse> AllWarehouse
        {
            get
            {
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.LoadAll<Warehouse>().Where(x => !x.IsDeleted);
                }
            }
        }

        public Item SelectedItem
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                using (var db = DatabaseDirector.GetDbInstance())
                {
                    AllSpecification = db.Table<Specification>().IndexQueryByKey("ItemUUID", _item.UUID).ToList();
                    SelectedSpecification = AllSpecification.FirstOrDefault();
                }
                _inventory.ItemUUID = _item.UUID;
                OnPropertyChanged("SelectedItem");
            }
        }

        public Specification SelectedSpecification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                _inventory.SpecificationUUID  = _specification != null ? _specification.UUID : null;
                OnPropertyChanged("SelectedSpecification");
            }
        }

        public Warehouse SelectedWarehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                if (_warehouse != null)
                    _inventory.WarehouseUUID = _warehouse.UUID;
                OnPropertyChanged("SelectedWarehouse");
            }
        }

        public int ItemCount
        {
            get
            {
                return _inventory.ItemCount;
            }
            set
            {
                _inventory.ItemCount = value;
                OnPropertyChanged("ItemCount");
            }
        }

        public InventoryEditorViewModel()
        {
            _inventory = new Inventory();
            IsEditMode = false;
        }

        public InventoryEditorViewModel(Inventory inventory)
        {
            _inventory = inventory;
            _item = _inventory.TraceItem();
            _specification = _inventory.TraceSpecification();
            _warehouse = _inventory.TraceWarehouse();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                _allSpecification = db.Table<Specification>().IndexQueryByKey("ItemUUID", _item.UUID).ToList();
            }
            IsEditMode = true;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}