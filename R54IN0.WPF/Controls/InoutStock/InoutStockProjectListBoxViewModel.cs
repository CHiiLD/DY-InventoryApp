﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InoutStockProjectListBoxViewModel : ICollectionViewModel<Observable<Project>>, INotifyPropertyChanged
    {
        ObservableCollection<Observable<Project>> _items;
        Observable<Project> _selectedItem;

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

        public InoutStockProjectListBoxViewModel()
        {
            var ofd = ObservableFieldDirector.GetInstance();
            var list = ofd.CreateList<Project>();
            Items = new ObservableCollection<Observable<Project>>(list);
        }

        public ObservableCollection<Observable<Project>> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                NotifyPropertyChanged("Items");
            }
        }

        public Observable<Project> SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }
        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
