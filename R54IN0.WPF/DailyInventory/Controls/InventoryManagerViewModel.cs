using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InventoryManagerViewModel : INotifyPropertyChanged
    {
        Observable<Product> _product;
        private string _specification;
        private string _memo;
        private ObservableCollection<Observable<Maker>> _makerList;
        private ObservableCollection<Observable<Measure>> _measureList;
        private string _makerText;
        private string _measureText;
        private Observable<Maker> _maker;
        private Observable<Measure> _measure;
        InventoryManagerDialog _control;

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
            Insert();
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
            var maker = Maker;
            var measure = Measure;

            if (maker == null && MakerText != null)
            {
                maker = new Observable<Maker>(MakerText);
                DataDirector.GetInstance().AddField(maker);
            }
            if (measure == null && MeasureText != null)
            {
                measure = new Observable<Measure>(MeasureText);
                DataDirector.GetInstance().AddField(measure);
            }

            ObservableInventory inventory = new ObservableInventory(_product, Specification, 0, Memo, maker, measure);
            DataDirector.GetInstance().AddInventory(inventory);

            return inventory;
        }
    }
}