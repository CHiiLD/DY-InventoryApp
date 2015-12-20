using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace R54IN0
{
    public class EditorViewModel<T> : INotifyPropertyChanged where T : class, IRecord, new()
    {
        T _inventory;
        IEnumerable<IFieldWrapper> _allSpecification;
        IFieldWrapper _item;
        IFieldWrapper _warehouse;
        IFieldWrapper _specification;

        public event PropertyChangedEventHandler PropertyChanged;
        public EditorModelViewAction Action { get; set; }

        public T Inventory
        {
            get
            {
                return _inventory;
            }
        }

        public IEnumerable<IFieldWrapper> AllItem
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Item>();
            }
        }

        public IEnumerable<IFieldWrapper> AllSpecification
        {
            get
            {
                return _allSpecification == null ? null : _allSpecification.Where(x => !x.IsDeleted);
            }
            set
            {
                _allSpecification = value;
                OnPropertyChanged("AllSpecification");
            }
        }

        public IEnumerable<IFieldWrapper> AllWarehouse
        {
            get
            {
                return FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Warehouse>();
            }
        }

        public IFieldWrapper SelectedItem
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                if (_item != null)
                {
                    ObservableCollection<IFieldWrapper> specColl = FieldPipeCollectionDirector.GetInstance().LoadEnablePipe<Specification>();
                    IEnumerable<IFieldWrapper> itemSpecColl = specColl.Where(x => ((Specification)x.Field).ItemUUID == _item.Field.UUID);
                    AllSpecification = itemSpecColl;
                    SelectedSpecification = AllSpecification.FirstOrDefault();
                    _inventory.ItemUUID = _item.Field.UUID;
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        public IFieldWrapper SelectedSpecification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                _inventory.SpecificationUUID  = _specification != null ? _specification.Field.UUID : null;
                OnPropertyChanged("SelectedSpecification");
            }
        }

        public IFieldWrapper SelectedWarehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                if (_warehouse != null)
                    _inventory.WarehouseUUID = _warehouse.Field.UUID;
                OnPropertyChanged("SelectedWarehouse");
            }
        }

        public int ItemCount
        {
            get
            {
                return _inventory.ItemCount;
            }
            set
            {
                _inventory.ItemCount = value;
                OnPropertyChanged("ItemCount");
            }
        }

        public EditorViewModel()
        {
            _inventory = new T();
            Action = EditorModelViewAction.ADD;
        }

        public EditorViewModel(T inventory)
        {
            _inventory = inventory;
            string tempSpecUUID = _inventory.SpecificationUUID; //SelectItem 속성에 값을 대입할 경우 _inventory.Sepc..UUID가 업데이트됨
            FieldPipeCollectionDirector fcd = FieldPipeCollectionDirector.GetInstance();

            SelectedItem = fcd.LoadPipe<Item>().Where(x => x.Field.UUID == _inventory.ItemUUID).SingleOrDefault();
            SelectedSpecification = fcd.LoadPipe<Specification>().Where(x => x.Field.UUID == tempSpecUUID).SingleOrDefault();
            SelectedWarehouse = fcd.LoadPipe<Warehouse>().Where(x => x.Field.UUID == _inventory.WarehouseUUID).SingleOrDefault();

            Action = EditorModelViewAction.EDIT;
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
