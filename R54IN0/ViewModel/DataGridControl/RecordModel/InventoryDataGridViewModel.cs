using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryDataGridViewModel : FinderViewModelMediatorColleague, IUpdateNewItems
    {
        public ObservableCollection<InventoryPipe> Items { get; set; }
        public InventoryPipe SelectedItem { get; set; }

        public InventoryDataGridViewModel() : base(FinderViewModelMediator.GetInstance())
        {
            var pipes = InventoryPipeCollectionDirector.GetInstance().LoadPipe();
            Items = new ObservableCollection<InventoryPipe>(pipes);
            SelectedItem = Items.FirstOrDefault();
        }

        public IEnumerable<object> LoadPipe()
        {
            return InventoryPipeCollectionDirector.GetInstance().LoadPipe();
        }

        public void UpdateNewItems(IEnumerable<object> items)
        {
            Items.Clear();
            foreach (var i in items)
                Items.Add(i as InventoryPipe);
            SelectedItem = Items.FirstOrDefault();
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Inven.Delete<Inventory>();
                InventoryPipeCollectionDirector.GetInstance().Remove(SelectedItem);
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }
        public void AddNewItem(Inventory inventory)
        {
            Check(inventory);

            RemoveOverlap(inventory.SpecificationUUID);

            InventoryPipe inventoryPipe = new InventoryPipe(inventory);
            InventoryPipeCollectionDirector.GetInstance().Add(inventoryPipe);
            Items.Add(inventoryPipe);
            SelectedItem = inventoryPipe;

            inventory.Save<Inventory>();
        }
        public void ReplaceItem(Inventory inventory)
        {
            Check(inventory);

            RemoveOverlap(inventory.SpecificationUUID);

            InventoryPipe oldPipe = Items.Where(x => x.Inven.UUID == inventory.UUID).Single();
            int idx = Items.IndexOf(oldPipe);
            Items.RemoveAt(idx);
            InventoryPipe newPipe = new InventoryPipe(inventory);
            Items.Insert(idx, newPipe);
            SelectedItem = newPipe;

            inventory.Save<Inventory>();
        }

        void Check(Inventory inventory)
        {
            if (inventory.ItemUUID == null)
                throw new ArgumentException("Item uuid 정보가 null");
            if (inventory.SpecificationUUID == null)
                throw new ArgumentException("specfication uuid 정보가 null");
        }

        void RemoveOverlap(string specificationUUID)
        {
            IEnumerable<InventoryPipe> overlaps = Items.
                Where(x => x.Inven.SpecificationUUID.CompareTo(specificationUUID) == 0);
            InventoryPipe overlap = overlaps.SingleOrDefault();
            if (overlap != null)
                Items.Remove(overlap);
        }
    }
}