using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InoutStockRegisterViewModel : ObservableInoutStock
    {
        public InoutStockRegisterViewModel() : base()
        {

        }

        public InoutStockRegisterViewModel(InoutStockFormat stockFormat) : base(stockFormat)
        {

        }

        public override void NotifyPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
