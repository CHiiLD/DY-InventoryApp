using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class InventoryEditorViewModel
    {
        InventoryPipe _inventoryPipe;

        public InventoryEditorViewModel()
        {
            _inventoryPipe = new InventoryPipe();
        }

        public InventoryEditorViewModel(InventoryPipe inventoryPipe)
        {
            _inventoryPipe = new InventoryPipe(inventoryPipe);
        }

        public InventoryPipe InventoryPipe
        {
            get
            {
                return _inventoryPipe;
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
                if (SelectedItem == null)
                    return null;

                using (var db = DatabaseDirector.GetDbInstance())
                {
                    return db.Table<Specification>().IndexQueryByKey("ItemUUID", SelectedItem.UUID).ToList();
                }
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
                return _inventoryPipe.Item;
            }
            set
            {
                _inventoryPipe.Item = value;
            }
        }

        public Specification SelectedSpecification
        {
            get
            {
                return _inventoryPipe.Specification;
            }
            set
            {
                _inventoryPipe.Specification = value;
            }
        }

        public Warehouse SelectedWarehouse
        {
            get
            {
                return _inventoryPipe.Warehouse;
            }
            set
            {
                _inventoryPipe.Warehouse = value;
            }
        }

        public int ItemCount
        {
            get
            {
                return _inventoryPipe.ItemCount;
            }
            set
            {
                _inventoryPipe.ItemCount = value;
            }
        }
    }
}