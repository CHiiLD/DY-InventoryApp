using System.Collections.Generic;

namespace R54IN0
{
    /// <summary>
    /// 품목
    /// </summary>
    public class Item : IField
    {
        public string UUID { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string MeasureUUID { get; set; }
        public string CurrencyUUID { get; set; }
        public string MakerUUID { get; set; }

        public Item()
        {

        }

        public Item(Item thiz)
        {
            UUID = thiz.UUID;
            Name = thiz.Name;
            IsDeleted = thiz.IsDeleted;
            MeasureUUID = thiz.MeasureUUID;
            CurrencyUUID = thiz.CurrencyUUID;
            MakerUUID = thiz.MakerUUID;
        }
    }
}