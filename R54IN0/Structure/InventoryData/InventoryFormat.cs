namespace R54IN0
{
    public class InventoryFormat : IID
    {
        /// <summary>
        /// 범용식별자
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 제품의 규격 이름
        /// </summary>
        public virtual string Specification { get; set; }

        /// <summary>
        /// 적재 수량
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public virtual string Memo { get; set; }

        /// <summary>
        /// 제품 범용식별자
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 제조사 범용식별자
        /// </summary>
        public string MakerID { get; set; }

        /// <summary>
        /// 단위 범위식별자
        /// </summary>
        public string MeasureID { get; set; }

        public InventoryFormat()
        {
        }

        public InventoryFormat(InventoryFormat thiz)
        {
            ID = thiz.ID;
            Specification = thiz.Specification;
            Quantity = thiz.Quantity;
            Memo = thiz.Memo;
            ProductID = thiz.ProductID;
            MakerID = thiz.MakerID;
            MeasureID = thiz.MeasureID;
        }
    }
}