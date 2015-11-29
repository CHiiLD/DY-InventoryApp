using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class SpecificationPipe : FieldPipe<Specification>
    {
        public SpecificationPipe(Specification spec)
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
