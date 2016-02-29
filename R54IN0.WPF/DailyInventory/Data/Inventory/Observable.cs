using System;
using System.ComponentModel;

namespace R54IN0.WPF
{
    public class Observable<FieldT> : IObservableField, INotifyPropertyChanged, IUpdateLock where FieldT : class, IField, new()
    {
        private FieldT _t;
        private bool _canUpdate = true;

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

        public Observable(FieldT field)
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
                throw new NotSupportedException();
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
                if (_t.Name != value)
                {
                    _t.Name = value;
                    NotifyPropertyChanged("Name");
                }
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
                Field = (FieldT)value;
            }
        }

        public virtual FieldT Field
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

        public bool UpdateLock
        {
            get
            {
                return _canUpdate;
            }
            set
            {
                _canUpdate = value;
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));

            if (string.IsNullOrEmpty(name))
                return;

            if (name == nameof(ID))
                throw new Exception();

            if (ID == null)
                throw new Exception("ID must not null.");
            else if (UpdateLock)
                DataDirector.GetInstance().Db.Update<FieldT>(Field);
        }
    }
}