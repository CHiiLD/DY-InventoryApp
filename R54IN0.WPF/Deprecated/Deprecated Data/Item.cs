namespace R54IN0
{
    /// <summary>
    /// 품목
    /// </summary>
    public class Item : IField
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string MeasureID { get; set; }
        public string CurrencyID { get; set; }
        public string MakerID { get; set; }

        public Item()
        {
        }

        public Item(Item thiz)
        {
            ID = thiz.ID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
            MeasureID = thiz.MeasureID;
            CurrencyID = thiz.CurrencyID;
            MakerID = thiz.MakerID;
        }
    }
}