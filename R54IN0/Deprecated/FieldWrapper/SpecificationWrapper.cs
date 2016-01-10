namespace R54IN0
{
    public class SpecificationWrapper : Observable<Specification>
    {
        public SpecificationWrapper() : base()
        {
        }

        public SpecificationWrapper(Specification spec)
            : base(spec)
        {
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return Field.PurchaseUnitPrice;
            }
            set
            {
                Field.PurchaseUnitPrice = value;
                Field.Save<Specification>();
                OnPropertyChanged("PurchaseUnitPrice");
            }
        }

        public decimal SalesUnitPrice
        {
            get
            {
                return Field.SalesUnitPrice;
            }
            set
            {
                Field.SalesUnitPrice = value;
                Field.Save<Specification>();
                OnPropertyChanged("SalesUnitPrice");
            }
        }

        public string Remark
        {
            get
            {
                return Field.Remark;
            }
            set
            {
                Field.Remark = value;
                Field.Save<Specification>();
                OnPropertyChanged("Remark");
            }
        }
    }
}