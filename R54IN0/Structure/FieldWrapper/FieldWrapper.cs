using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class FieldWrapper<T> : IFieldWrapper, INotifyPropertyChanged where T : class, IField
    {
        T _field;
        event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        public FieldWrapper()
        {

        }

        public FieldWrapper(T field)
        {
            _field = field;
        }

        IField IFieldWrapper.Field
        {
            get
            {
                return Field;
            }
            set
            {
                Field = (T)value;
            }
        }

        public virtual T Field
        {
            get
            {
                return _field;
            }
            set
            {
                _field = value;
                OnPropertyChanged("");
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
                _field.Save<T>();
                OnPropertyChanged("Name");
            }
        }

        public string UUID
        {
            get
            {
                return _field.UUID;
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
                _field.Save<T>();
                OnPropertyChanged("IsDeleted");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
