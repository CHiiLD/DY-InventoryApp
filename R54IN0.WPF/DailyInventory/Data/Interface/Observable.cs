using System.ComponentModel;

namespace R54IN0
{
    public class Observable<T> : IObservableField, INotifyPropertyChanged where T : class, IField, new()
    {
        private T _t;

        private event PropertyChangedEventHandler _propertyChanged;

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
                NotifyPropertyChanged("Name");
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
                NotifyPropertyChanged("IsDeleted");
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
                NotifyPropertyChanged(string.Empty);
            }
        }

        protected async void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));

            if (ID == null)
                await DbAdapter.GetInstance().InsertAsync(Field);
            else
                await DbAdapter.GetInstance().UpdateAsync(Field, name);
        }
    }
}