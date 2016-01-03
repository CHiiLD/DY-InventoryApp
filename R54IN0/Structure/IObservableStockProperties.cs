using System;
using System.ComponentModel;

namespace R54IN0
{
    public interface IObservableStockProperties : INotifyPropertyChanged
    {
        string ID { get; set; }
        string Memo { get; set; }
        int Quantity { get; set; }
        decimal UnitPrice { get; set; }
        StockType StockType { get; set; }
        IObservableInventoryProperties Inventory { get; set; }
        DateTime Date { get; set; }
        Observable<Supplier> Supplier { get; set; }
        Observable<Customer> Customer { get; set; }
        Observable<Project> Project { get; set; }
    }
}