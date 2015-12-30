using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class Observable<T> : IObservableField, INotifyPropertyChanged where T : class, IField, new()
    {
        T _t;
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

        public Observable()
        {
            _t = new T();
        }

        public Observable(T field)
        {
            _t = field;
        }

        public string ID
        {
            get
            {
                return _t.ID;
            }
            set
            {
                _t.ID = value;
            }
        }

        public string Name
        {
            get
            {
                return _t.Name;
            }
            set
            {
                _t.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public bool IsDeleted
        {
            get
            {
                return _t.IsDeleted;
            }
            set
            {
                _t.IsDeleted = value;
                OnPropertyChanged("IsDeleted");
            }
        }

        IField IObservableField.Field
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
                return _t;
            }
            set
            {
                _t = value;
                OnPropertyChanged("");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
            if (!string.IsNullOrEmpty(name))
                _t.Save<T>();
        }
    }
}