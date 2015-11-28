using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0
{
    public class SpecificationPipe : FieldPipe
    {
        Specification _spec;

        public SpecificationPipe(Specification spec) : base()
        {
            _spec = spec;
        }

        public Specification Specification
        {
            get
            {
                return _spec;
            }
        }

        public override string Name
        {
            get
            {
                return _spec.Name;
            }
            set
            {
                if (_spec.Name != value)
                {
                    _spec.Name = value;
                    _spec.Save<Specification>();
                }
                OnPropertyChanged("Name");
            }
        }

        public decimal PurchaseUnitPrice
        {
            get
            {
                return _spec.PurchaseUnitPrice;
            }
            set
            {
                if (_spec.PurchaseUnitPrice != value)
                {
                    _spec.PurchaseUnitPrice = value;
                    _spec.Save<Specification>();
                }
                OnPropertyChanged("PurchaseUnitPrice");
            }
        }

        public decimal SalesUnitPrice
        {
            get
            {
                return _spec.SalesUnitPrice;
            }
            set
            {
                if (_spec.SalesUnitPrice != value)
                {
                    _spec.SalesUnitPrice = value;
                    _spec.Save<Specification>();
                }
                OnPropertyChanged("SalesUnitPrice");
            }
        }
    }
}
