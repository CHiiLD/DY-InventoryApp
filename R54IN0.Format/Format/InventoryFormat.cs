namespace R54IN0
{
    public class InventoryFormat : IID, IInventoryFormat
    {
        public const int LENGTH_LIMIT = 256;
        private string _specification;
        private string _memo;

        /// <summary>
        /// 범용식별자
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 제품의 규격 이름
        /// </summary>
        public virtual string Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = this.CutString(value, LENGTH_LIMIT);
            }
        }

        /// <summary>
        /// 적재 수량
        /// </summary>
        public virtual int Quantity { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public virtual string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = this.CutString(value, LENGTH_LIMIT);
            }
        }

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