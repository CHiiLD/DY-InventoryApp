using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InOutStockDataGridViewModel
    {
        StockType _stockType;

        public ObservableCollection<InOutStockPipe> Items { get; set; }
        public InOutStockPipe SelectedItem { get; set; }

        public InOutStockDataGridViewModel(StockType type)
        {
            if (type == StockType.NONE)
                throw new ArgumentException();
            _stockType = type;
            Items = InOutStockPipeCollectionDirector.GetInstance().LoadPipe(_stockType);
            SelectedItem = Items.FirstOrDefault();
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Inven.Delete<InOutStock>();
                InOutStockPipeCollectionDirector.GetInstance().RemoveItem(SelectedItem);
                SelectedItem = Items.FirstOrDefault();
            }
        }

        void Check(InOutStock ioStock)
        {
            if (ioStock.ItemUUID == null)
                throw new ArgumentException("Item uuid 정보가 null");
            if (ioStock.SpecificationUUID == null)
                throw new ArgumentException("specfication uuid 정보가 null");
        }

        void AddInventoryItemCount(InOutStock ioStock, int count)
        {
            InventoryPipe invenPipe = InventoryPipeCollectionDirector.GetInstance().LoadPipe()
                .Where(x => x.Specification.Field.UUID == ioStock.SpecificationUUID).SingleOrDefault();

            if (invenPipe != null)
                invenPipe.ItemCount += count;
        }

        public void AddNewItem(InOutStock newStock)
        {
            Check(newStock);

            newStock.Save<InOutStock>();
            InOutStockPipe ioStockPipe = new InOutStockPipe(newStock);
            InOutStockPipeCollectionDirector.GetInstance().AddNewItem(ioStockPipe);
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
    }
}