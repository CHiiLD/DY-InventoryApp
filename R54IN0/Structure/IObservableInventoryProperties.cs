using System.ComponentModel;

namespace R54IN0
{
    public interface IObservableInventoryProperties : INotifyPropertyChanged
    {
        string ID { get; set; }
        string Memo { get; set; }
        int Quantity { get; set; }
        string Specification { get; set; }
        Observable<Product> Product { get; set; }
        Observable<Measure> Measure { get; set; }
        Observable<Maker> Maker { get; set; }
    }
}