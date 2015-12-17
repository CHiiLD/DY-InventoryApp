using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InOutStockDataGridViewModel : FinderViewModelMediatorColleague, IUpdateNewItems
    {
        StockType _stockType;

        public ObservableCollection<InOutStockPipe> Items { get; set; }
        public InOutStockPipe SelectedItem { get; set; }

        public InOutStockDataGridViewModel(StockType type) : base(FinderViewModelMediator.GetInstance())
        {
            if (type == StockType.NONE)
                throw new ArgumentException();
            _stockType = type;
            var items = InOutStockPipeCollectionDirector.GetInstance().NewPipe(_stockType);
            Items = new ObservableCollection<InOutStockPipe>(items);
            SelectedItem = Items.FirstOrDefault();
        }

        public IEnumerable<object> LoadPipe()
        {
            return InOutStockPipeCollectionDirector.GetInstance().NewPipe(_stockType);
        }

        public void UpdateNewItems(IEnumerable<object> items)
        {
            Items.Clear();
            foreach (var i in items)
                Items.Add(i as InOutStockPipe);
            SelectedItem = Items.FirstOrDefault();
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Inven.Delete<InOutStock>();
                InOutStockPipeCollectionDirector.GetInstance().Remove(SelectedItem);
                Items.Remove(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        public void AddNewItem(InOutStock newStock)
        {
            Check(newStock);

            newStock.Save<InOutStock>();
            InOutStockPipe ioStockPipe = new InOutStockPipe(newStock);
            InOutStockPipeCollectionDirector.GetInstance().Add(ioStockPipe);
            Items.Add(ioStockPipe);
            SelectedItem = ioStockPipe;

            switch (newStock.StockType)
            {
                case StockType.IN:
                    AddInventoryItemCount(newStock, newStock.ItemCount);
                    break;
                case StockType.OUT:
                    AddInventoryItemCount(newStock, -newStock.ItemCount);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void ReplaceItem(InOutStock ioStock)
        {
            Check(ioStock);

            ioStock.Save<InOutStock>();
            InOutStockPipe oldPipe = Items.Where(x => x.Inven.UUID == ioStock.UUID).Single();
            int count = oldPipe.ItemCount;
            int idx = Items.IndexOf(oldPipe);
            Items.RemoveAt(idx);
            InOutStockPipe newPipe = new InOutStockPipe(ioStock);
            Items.Insert(idx, newPipe);
            SelectedItem = newPipe;

            switch (ioStock.StockType)
            {
                case StockType.IN:
                    AddInventoryItemCount(ioStock, count - ioStock.ItemCount);
                    break;
                case StockType.OUT:
                    AddInventoryItemCount(ioStock, -(count - ioStock.ItemCount));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        void Check(InOutStock ioStock)
        {
            if (ioStock.ItemUUID == null)
                throw new ArgumentException("ITEM UUID 정보가 NULL");
            if (ioStock.SpecificationUUID == null)
                throw new ArgumentException("SPECFICATION UUID 정보가 NULL");
        }

        void AddInventoryItemCount(InOutStock ioStock, int count)
        {
            InventoryPipe invenPipe = InventoryPipeCollectionDirector.GetInstance().LoadPipe()
                .Where(x => x.Specification.Field.UUID == ioStock.SpecificationUUID).SingleOrDefault();

            if (invenPipe != null)
                invenPipe.ItemCount += count;
        }
    }
}