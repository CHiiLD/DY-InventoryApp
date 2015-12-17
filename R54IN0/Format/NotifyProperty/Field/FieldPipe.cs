using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace R54IN0
{
    public class FieldPipe<T> : IFieldPipe, INotifyPropertyChanged where T : class, IField
    {
        T _field;

        public event PropertyChangedEventHandler PropertyChanged;
        public FieldPipe(T field)
        {
            _field = field;
        }

        IField IFieldPipe.Field
        {
            get
            {
                return _field;
            }
            set
            {

            }
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
            set
            {
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
        
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
