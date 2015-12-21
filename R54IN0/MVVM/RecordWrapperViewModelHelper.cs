using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class RecordWrapperViewModelHelper<IRecordWrapperT, IRecordT> : INotifyPropertyChanged
        where IRecordWrapperT : class, IRecordWrapper, new()
        where IRecordT : class, IRecord, new()
    {
        ViewModelObserver<IRecordWrapperT> _viewModelObserver;
        ItemWrapper _item;
        IEnumerable<SpecificationWrapper> _allSpecification;
        SpecificationWrapper _selectedSpecification;
        FieldWrapper<Warehouse> _selectedWarehouse;

        protected IRecord record;

        public RecordWrapperViewModelHelper(ViewModelObserver<IRecordWrapperT> viewModelObserver, IRecordWrapperT target = null)
        {
            if (target != null && !viewModelObserver.Items.Contains(target))
                throw new ArgumentOutOfRangeException();
            _viewModelObserver = viewModelObserver;
            var fwd = FieldWrapperDirector.GetInstance();
            AllWarehouse = fwd.CreateCollection<Warehouse, FieldWrapper<Warehouse>>().Where(x => !x.IsDeleted);

            if (target != null)
            {
                AllItem = new ItemWrapper[] { target.Item };
                AllSpecification = new SpecificationWrapper[] { target.Specification };

                _item = target.Item;
                _selectedSpecification = target.Specification;
                _selectedWarehouse = target.Warehouse;
                record = target.Record.Clone() as IRecord;
            }
            else
            {
                IRecordT record = new IRecordT();
                this.record = record;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<ItemWrapper> AllItem
        {
            get;
            set;
        }

        public IEnumerable<SpecificationWrapper> AllSpecification
        {
            get
            {
                return _allSpecification;
            }
            set
            {
                _allSpecification = value;
                OnPropertyChanged("AllSpecification");
            }
        }

        public IEnumerable<FieldWrapper<Warehouse>> AllWarehouse
        {
            get;
            set;
        }

        public virtual ItemWrapper SelectedItem
        {
            get
            {
                return _item;
            }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    record.ItemUUID = value.UUID;
                    UpdateAllSpecificationProperty(_item);
                }
                OnPropertyChanged("SelectedItem");
            }
        }

        public SpecificationWrapper SelectedSpecification
        {
            get
            {
                return _selectedSpecification;
            }
            set
            {
                _selectedSpecification = value;
                //_recordWrapper.Specification = _selectedSpecification;
                record.SpecificationUUID = value.UUID;
                OnPropertyChanged("SelectedSpecification");
            }
        }

        public FieldWrapper<Warehouse> SelectedWarehouse
        {
            get
            {
                return _selectedWarehouse;
            }
            set
            {
                _selectedWarehouse = value;
                //_recordWrapper.Warehouse = _selectedWarehouse;
                record.WarehouseUUID = value.UUID;
                OnPropertyChanged("SelectedWarehouse");
            }
        }

        public int ItemCount
        {
            get
            {
                return record.ItemCount;
            }
            set
            {
                record.ItemCount = value;
                OnPropertyChanged("ItemCount");
            }
        }

        public void Update()
        {
            if (SelectedItem == null)
                throw new Exception();
            var wrapper = new IRecordWrapperT();
            wrapper.Record = record;
            _viewModelObserver.Add(wrapper);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void UpdateAllSpecificationProperty(ItemWrapper itemw)
        {
            var fwd = FieldWrapperDirector.GetInstance();
            var specws = fwd.CreateCollection<Specification, SpecificationWrapper>();
            AllSpecification = specws.Where(x => !x.IsDeleted && x.Field.ItemUUID == _item.UUID);
        }
    }
}