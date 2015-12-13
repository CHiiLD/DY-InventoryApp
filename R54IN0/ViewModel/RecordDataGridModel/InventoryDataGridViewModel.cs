using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryDataGridViewModel : AFinderViewModelMediatorColleague
    {
        public ObservableCollection<InventoryPipe> Items { get; set; }
        public InventoryPipe SelectedItem { get; set; }

        public InventoryDataGridViewModel() : base(FinderViewModelMediator.GetInstance())
        {
            var pipes = InventoryPipeCollectionDirector.GetInstance().LoadPipe();
            Items = new ObservableCollection<InventoryPipe>(pipes);
            SelectedItem = Items.FirstOrDefault();
        }

        public void ChangeInventoryItems(IEnumerable<InventoryPipe> items)
        {
            Items.Clear();
            foreach (var i in items)
                Items.Add(i);
            SelectedItem = Items.FirstOrDefault();
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Inven.Delete<Inventory>();
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        void Check(Inventory inventory)
        {
            if (inventory.ItemUUID == null)
                throw new ArgumentException("Item uuid 정보가 null");
            if (inventory.SpecificationUUID == null)
                throw new ArgumentException("specfication uuid 정보가 null");
        }

        private void RemoveOverlap(Inventory inventory)
        {
            InventoryPipe overlap = Items.
                Where(x => x.Inven.UUID != inventory.UUID &&
                x.Inven.SpecificationUUID == inventory.SpecificationUUID).SingleOrDefault();
            if (overlap != null)
                Items.Remove(overlap);
        }

        public void AddNewItem(Inventory inventory)
        {
            Check(inventory);

            RemoveOverlap(inventory.SpecificationUUID);

            InventoryPipe inventoryPipe = new InventoryPipe(inventory);
            Items.Add(inventoryPipe);
            SelectedItem = inventoryPipe;

            inventory.Save<Inventory>();
        }

        private void RemoveOverlap(string specificationUUID)
        {
            IEnumerable<InventoryPipe> overlaps = Items.
                Where(x => x.Inven.SpecificationUUID.CompareTo(specificationUUID) == 0); //.SingleOrDefault();
            InventoryPipe overlap = overlaps.SingleOrDefault();
            if (overlap != null)
                Items.Remove(overlap);
        }

        public void ReplaceItem(Inventory inventory)
        {
            Check(inventory);

            RemoveOverlap(inventory.SpecificationUUID);

            InventoryPipe old = Items.Where(x => x.Inven.UUID == inventory.UUID).Single();
            int idx = Items.IndexOf(old);
            Items.RemoveAt(idx);
            InventoryPipe newPipe = new InventoryPipe(inventory);
            Items.Insert(idx, newPipe);
            SelectedItem = newPipe;

            inventory.Save<Inventory>();
        }
    }
}