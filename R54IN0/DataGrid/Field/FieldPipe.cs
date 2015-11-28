using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class FieldPipe<T> : INotifyPropertyChanged where T : class, IField
    {
        T _field;

        public event PropertyChangedEventHandler PropertyChanged;

        public FieldPipe(T field)
        {
            _field = field;
        }

        public T Field
        {
            get
            {
                return _field;
            }
        }

        public string Name
        {
            get
            {
                return _field.Name;
            }
            set
            {
                _field.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _field.IsDeleted;
            }
            set
            {
                _field.IsDeleted = value;
                OnPropertyChanged("IsDelete");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
