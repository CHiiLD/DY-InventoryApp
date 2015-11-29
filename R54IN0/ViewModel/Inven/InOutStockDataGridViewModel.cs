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
            Items = new ObservableCollection<InOutStockPipe>();
            _stockType = type;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                IEnumerable<InOutStock> stocks = db.LoadAll<InOutStock>();
                if (_stockType.HasFlag(StockType.IN))
                {
                    foreach (var stock in stocks.Where(x => x.StockType == StockType.IN))
                        Items.Add(new InOutStockPipe(stock));
                }
                if (_stockType.HasFlag(StockType.OUT))
                {
                    foreach (var stock in stocks.Where(x => x.StockType == StockType.OUT))
                        Items.Add(new InOutStockPipe(stock));
                }
            }
            SelectedItem = Items.FirstOrDefault();
        }

        public void ChangeInventoryItems(IEnumerable<InOutStockPipe> items)
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
                SelectedItem.Inven.Delete<InOutStock>();
                Items.Remove(SelectedItem);
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
            Inventory inven = null;
            using (var db = DatabaseDirector.GetDbInstance())
            {
                List<Inventory> invens = db.Table<Inventory>().IndexQueryByKey("SpecificationUUID", ioStock.SpecificationUUID).ToList();
                inven = invens.SingleOrDefault();
            }
            if (inven != null)
            {
                ioStock.ItemCount += inven.ItemCount;
                inven.Save<Inventory>();
            }
        }

        public void Add(InOutStock ioStock)
        {
            Check(ioStock);

            ioStock.Save<InOutStock>();
            InOutStockPipe ioStockPipe = new InOutStockPipe(ioStock);
            Items.Add(ioStockPipe);
            SelectedItem = ioStockPipe;

            switch (ioStock.StockType)
            {
                case StockType.IN: AddInventoryItemCount(ioStock, ioStock.ItemCount);
                    break;
                case StockType.OUT: AddInventoryItemCount(ioStock, -ioStock.ItemCount);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public void Replace(InOutStock ioStock)
        {
            Check(ioStock);

            InOutStockPipe oldPipe = Items.Where(x => x.Inven.UUID == ioStock.UUID).Single();
            int count = oldPipe.ItemCount;
            int idx = Items.IndexOf(oldPipe);
            Items.RemoveAt(idx);
            InOutStockPipe newPipe = new InOutStockPipe(ioStock);
            Items.Insert(idx, newPipe);
            SelectedItem = newPipe;

            switch (ioStock.StockType)
            {
                case StockType.IN: AddInventoryItemCount(ioStock, count - ioStock.ItemCount);
                    break;
                case StockType.OUT: AddInventoryItemCount(ioStock, -(count - ioStock.ItemCount));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
