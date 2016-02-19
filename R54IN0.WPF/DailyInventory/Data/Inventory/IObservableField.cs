namespace R54IN0.WPF
{
    public interface IObservableField
    {
        string ID { get; set; }
        string Name { get; set; }
        IField Field { get; set; }
    }
}