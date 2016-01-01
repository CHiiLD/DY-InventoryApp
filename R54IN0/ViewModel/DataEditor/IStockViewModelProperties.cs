using System;

namespace R54IN0
{
    public interface IStockViewModelProperties : IInventoryViewModelProperties
    {
        string Remark { get; set; }
        Observable<Employee> Employee { get; set; }
        StockType StockType { get; set; }
        DateTime Date { get; set; }
        ClientWrapper Client { get; set; }
    }
}