using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace R54IN0.WPF
{
    public class InventoryManagerViewModel : INotifyPropertyChanged
    {
        private Observable<Product> _product;
        private string _specification;
        private string _memo;
        private ObservableCollection<Observable<Maker>> _makerList;
        private ObservableCollection<Observable<Measure>> _measureList;
        private string _makerText;
        private string _measureText;
        private Observable<Maker> _maker;
        private Observable<Measure> _measure;
        private InventoryManagerDialog _control;
        private ObservableInventory _target;

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

        public InventoryManagerViewModel(Observable<Product> product)
        {
            if (product == null)
                throw new NotSupportedException();

            _product = product;

            RecordCommand = new RelayCommand(ExecuteRecordCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            var idc = DataDirector.GetInstance();
            _makerList = new ObservableCollection<Observable<Maker>>(idc.CopyFields<Maker>());
            _measureList = new ObservableCollection<Observable<Measure>>(idc.CopyFields<Measure>());
        }

        public InventoryManagerViewModel(InventoryManagerDialog dialog, Observable<Product> product) : this(product)
        {
            _control = dialog;
        }

        public InventoryManagerViewModel(ObservableInventory inventory)
            : this(inventory.Product)
        {
            _target = inventory;

            Specification = inventory.Specification;
            Memo = inventory.Memo;
            Maker = inventory.Maker;
            Measure = inventory.Measure;
        }

        public InventoryManagerViewModel(InventoryManagerDialog dialog, ObservableInventory inventory)
            : this(inventory)
        {
            _control = dialog;
        }

        public RelayCommand RecordCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public string ProductName
        {
            get
            {
                return _product.Name;
            }
        }

        public string Specification
        {
            get
            {
                return _specification;
            }
            set
            {
                _specification = value;
                NotifyPropertyChanged("SpecificationName");
            }
        }

        public string Memo
        {
            get
            {
                return _memo;
            }
            set
            {
                _memo = value;
                NotifyPropertyChanged("SpecificationMemo");
            }
        }

        public ObservableCollection<Observable<Maker>> MakerList
        {
            get
            {
                return _makerList;
            }
            set
            {
                _makerList = value;
                NotifyPropertyChanged("MakerList");
            }
        }

        public ObservableCollection<Observable<Measure>> MeasureList
        {
            get
            {
                return _measureList;
            }
            set
            {
                _measureList = value;
                NotifyPropertyChanged("MeasureList");
            }
        }

        public string MakerText
        {
            get
            {
                return _makerText;
            }
            set
            {
                _makerText = value;
                NotifyPropertyChanged("MakerText");
            }
        }

        public string MeasureText
        {
            get
            {
                return _measureText;
            }
            set
            {
                _measureText = value;
                NotifyPropertyChanged("MeasureText");
            }
        }

        public Observable<Maker> Maker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                NotifyPropertyChanged("Maker");
            }
        }

        public Observable<Measure> Measure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                NotifyPropertyChanged("Measure");
            }
        }

        private async void ExecuteRecordCommand()
        {
            if (_target == null)
                Insert();
            else
                Update();
            await _control.RequestCloseAsync();
        }

        private async void ExecuteCancelCommand()
        {
            await _control.RequestCloseAsync();
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public ObservableInventory Insert()
        {
            CreateBindingProperties();

            InventoryFormat format = CreateInventoryFormat();
            format.ID = Guid.NewGuid().ToString();
            ObservableInventory inventory = new ObservableInventory(format);
            DataDirector.GetInstance().AddInventory(inventory);

            return inventory;
        }

        public ObservableInventory Update()
        {
            ObservableInventory origin = _target;

            ModifyBindingProperties();
            CreateBindingProperties();

            InventoryFormat modify = CreateInventoryFormat();
            modify.ID = origin.ID;
            modify.Quantity = origin.Quantity;

            PropertyInfo[] properties = modify.GetType().GetProperties();
            foreach (PropertyInfo modifyProperty in properties)
            {
                if (modifyProperty.PropertyType.IsNotPublic)
                    continue;
                string pname = modifyProperty.Name;
                PropertyInfo originProperty = origin.GetType().GetProperty(pname);
                object v1 = originProperty.GetValue(origin);
                object v2 = modifyProperty.GetValue(modify);
                if (v1 != v2)
                    originProperty.SetValue(origin, v2);
            }
            return origin;
        }

        private void CreateBindingProperties()
        {
            var maker = Maker;
            var measure = Measure;

            if (maker == null && MakerText != null)
            {
                maker = new Observable<Maker>(MakerText);
                DataDirector.GetInstance().AddField(maker);
                Maker = maker;
            }
            if (measure == null && MeasureText != null)
            {
                measure = new Observable<Measure>(MeasureText);
                DataDirector.GetInstance().AddField(measure);
                Measure = measure;
            }
        }

        private void ModifyBindingProperties()
        {
            var maker = Maker;
            var measure = Measure;
            ObservableInventory origin = _target;

            if (origin.Maker != null && maker == null)
            {
                origin.Maker.Name = MakerText;
                Maker = origin.Maker;
            }
            if (origin.Measure != null && Measure == null)
            {
                origin.Measure.Name = MeasureText;
                Measure = origin.Measure;
            }
        }

        private InventoryFormat CreateInventoryFormat()
        {
            InventoryFormat format = new InventoryFormat();
            format.ProductID = _product.ID;
            if (Maker != null)
                format.MakerID = Maker.ID;
            if (Measure != null)
                format.MeasureID = Measure.ID;
            format.Specification = Specification;
            format.Memo = Memo;

            return format;
        }
    }
}