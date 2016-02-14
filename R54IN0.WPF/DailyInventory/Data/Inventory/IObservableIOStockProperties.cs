using System;
using System.ComponentModel;

namespace R54IN0
{
    public interface IObservableIOStockProperties : INotifyPropertyChanged, IPropertyChanged
    {
        string ID { get; set; }
        string Memo { get; set; }
        int Quantity { get; set; }
        int RemainingQuantity { get; set; }
        decimal UnitPrice { get; set; }
        IOStockType StockType { get; set; }
        IObservableInventoryProperties Inventory { get; set; }
        DateTime Date { get; set; }
        Observable<Supplier> Supplier { get; set; }
        Observable<Customer> Customer { get; set; }
        Observable<Project> Project { get; set; }
        Observable<Employee> Employee { get; set; }
        Observable<Warehouse> Warehouse { get; set; }
        IOStockFormat Format { get; set; }
    }
}