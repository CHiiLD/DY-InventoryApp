using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace R54IN0
{
    public class Observable<FieldT> : IObservableField, ISync, INotifyPropertyChanged where FieldT : class, IField, new()
    {
        private FieldT _t;

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
            _t = new FieldT();
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

        protected async void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));

            if (ID == null)
            {
                await DbAdapter.GetInstance().InsertAsync(Field);
                ObservableFieldDirector.GetInstance().Add<FieldT>(this);
            }
            else
            {
                await DbAdapter.GetInstance().UpdateAsync(Field, name);
            }
        }

        public async Task SyncDataFromServer()
        {
            FieldT field = await DbAdapter.GetInstance().SelectAsync<FieldT>(ID);
            if (field != null)
            {
                if (Name != field.Name)
                    Name = field.Name;
                if (IsDeleted != field.IsDeleted)
                    IsDeleted = field.IsDeleted;
            }
        }
    }
}