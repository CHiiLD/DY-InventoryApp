using R54IN0.WPF;
using System.ComponentModel;
using System.Threading.Tasks;
using System;

namespace R54IN0.WPF
{
    public class Observable<FieldT> : IObservableField, INotifyPropertyChanged, ICanUpdate where FieldT : class, IField, new()
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

        public Observable()
        {
            _t = new FieldT();
        }

        /// <summary>
        /// 이름 할당 및 자동 디비 저장
        /// </summary>
        /// <param name="name"></param>
        public Observable(string name) : this()
        {
            _t.Name = name;
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

        public bool CanUpdate
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

            if (ID == null)
                DataDirector.GetInstance().AddField(this);
            else if (CanUpdate)
                DataDirector.GetInstance().DB.Update(Field, name);
        }

        public void Refresh()
        {
            FieldT field = DataDirector.GetInstance().DB.Select<FieldT>(ID);
            if (field != null)
            {
                if (Name != field.Name)
                    Name = field.Name;
            }
        }
    }
}