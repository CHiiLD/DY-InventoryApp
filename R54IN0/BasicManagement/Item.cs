namespace DY.Inven
{
    /// <summary>
    /// 품목
    /// </summary>
    public class Item : IBasic
    {
        public string Name { get; set; }
        public string UUID { get; set; }
        public bool IsEnable { get; set; }
        public string MeasureUUID { get; set; }
        public string CurrencyUUID { get; set; }
    }
}