using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class WField<IFieldType> : INotifyPropertyChanged, IComparable where IFieldType : class, IField, new()
    {
        IFieldType _field;
        public event PropertyChangedEventHandler PropertyChanged;

        public WField(IFieldType field)
        {
            _field = field;
        }

        public IFieldType Field
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
                Field.Save<IFieldType>();
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
                Field.Save<IFieldType>();
                OnPropertyChanged("IsDeleted");
            }
        }

        public int CompareTo(object obj)
        {
            var another = obj as WField<IFieldType>;
            return string.Compare(_field.UUID, another._field.UUID);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
