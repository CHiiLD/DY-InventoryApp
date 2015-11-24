namespace R54IN0.Lib
{
    /// <summary>
    /// 품목
    /// </summary>
    public class Item : IBasic
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string MeasureUUID { get; set; }
        public string CurrencyUUID { get; set; }
    }
}