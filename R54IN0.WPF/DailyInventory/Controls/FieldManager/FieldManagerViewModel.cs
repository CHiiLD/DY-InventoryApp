using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace R54IN0.WPF
{
    public class FieldManagerViewModel : INotifyPropertyChanged, ICollectionViewModelObserver
    {
        private Observable<Customer> _customer;
        private Observable<Employee> _employee;
        private Observable<Measure> _measure;
        private Observable<Maker> _maker;
        private Observable<Supplier> _supplier;
        private Observable<Warehouse> _warehouse;
        private Observable<Project> _project;
        private TabItem _tabItem;

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

        public FieldManagerViewModel()
        {
            DataDirector ddr = DataDirector.GetInstance();

            AddNewItemCommand = new RelayCommand(ExecuteAddNewItemCommand, CanAdd);
            DeleteItemCommand = new RelayCommand(ExecuteDeleteItemCommand, CanDelete);

            MakerList = new ObservableCollection<Observable<Maker>>(ddr.CopyFields<Maker>());
            MeasureList = new ObservableCollection<Observable<Measure>>(ddr.CopyFields<Measure>());
            EmployeeList = new ObservableCollection<Observable<Employee>>(ddr.CopyFields<Employee>());
            CustomerList = new ObservableCollection<Observable<Customer>>(ddr.CopyFields<Customer>());
            SupplierList = new ObservableCollection<Observable<Supplier>>(ddr.CopyFields<Supplier>());
            WarehouseList = new ObservableCollection<Observable<Warehouse>>(ddr.CopyFields<Warehouse>());
            ProjectList = new ObservableCollection<Observable<Project>>(ddr.CopyFields<Project>());

            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        ~FieldManagerViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        public TabItem SelectedTabItem
        {
            get
            {
                return _tabItem;
            }
            set
            {
                _tabItem = value;
                NotifyPropertyChanged("SelectedTabItem");
            }
        }

        public RelayCommand AddNewItemCommand
        {
            get;
            private set;
        }

        public RelayCommand DeleteItemCommand
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Maker>> MakerList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Measure>> MeasureList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Employee>> EmployeeList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Customer>> CustomerList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Supplier>> SupplierList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Warehouse>> WarehouseList
        {
            get;
            private set;
        }

        public ObservableCollection<Observable<Project>> ProjectList
        {
            get;
            private set;
        }

        public Observable<Maker> SelectedMaker
        {
            get
            {
                return _maker;
            }
            set
            {
                _maker = value;
                NotifyPropertyChanged("SelectedMaker");
            }
        }

        public Observable<Measure> SelectedMeasure
        {
            get
            {
                return _measure;
            }
            set
            {
                _measure = value;
                NotifyPropertyChanged("SelectedMeasure");
            }
        }

        public Observable<Employee> SelectedEmployee
        {
            get
            {
                return _employee;
            }
            set
            {
                _employee = value;
                NotifyPropertyChanged("SelectedEmployee");
            }
        }

        public Observable<Customer> SelectedCustomer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                NotifyPropertyChanged("SelectedCustomer");
            }
        }

        public Observable<Supplier> SelectedSupplier
        {
            get
            {
                return _supplier;
            }
            set
            {
                _supplier = value;
                NotifyPropertyChanged("SelectedSupplier");
            }
        }

        public Observable<Warehouse> SelectedWarehouse
        {
            get
            {
                return _warehouse;
            }
            set
            {
                _warehouse = value;
                NotifyPropertyChanged("SelectedWarehouse");
            }
        }

        public Observable<Project> SelectedProject
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
                NotifyPropertyChanged("SelectedProject");
            }
        }

        private bool CanDelete()
        {
            if (SelectedTabItem == null)
                return false;

            IObservableField selectedObject = null;
            string header = SelectedTabItem.Header as string;
            switch (header)
            {
                case Maker.HEADER:
                    selectedObject = SelectedMaker;
                    break;
                case Measure.HEADER:
                    selectedObject = SelectedMeasure;
                    break;
                case Employee.HEADER:
                    selectedObject = SelectedEmployee;
                    break;
                case Customer.HEADER:
                    selectedObject = SelectedCustomer;
                    break;
                case Supplier.HEADER:
                    selectedObject = SelectedSupplier;
                    break;
                case Warehouse.HEADER:
                    selectedObject = SelectedWarehouse;
                    break;
                case Project.HEADER:
                    selectedObject = SelectedProject;
                    break;
            }
            return selectedObject != null;
        }

        private void ExecuteDeleteItemCommand()
        {
            IObservableField selectedObject = null;
            string header = SelectedTabItem.Header as string;
            switch (header)
            {
                case Maker.HEADER:
                    selectedObject = SelectedMaker;
                    break;
                case Measure.HEADER:
                    selectedObject = SelectedMeasure;
                    break;
                case Employee.HEADER:
                    selectedObject = SelectedEmployee;
                    break;
                case Customer.HEADER:
                    selectedObject = SelectedCustomer;
                    break;
                case Supplier.HEADER:
                    selectedObject = SelectedSupplier;
                    break;
                case Warehouse.HEADER:
                    selectedObject = SelectedWarehouse;
                    break;
                case Project.HEADER:
                    selectedObject = SelectedProject;
                    break;
                default:
                    throw new NotSupportedException();
            }
            RemoveField(selectedObject);
        }

        private bool CanAdd()
        {
            return SelectedTabItem != null;
        }

        private void ExecuteAddNewItemCommand()
        {
            IField newField = null;
            string header = SelectedTabItem.Header as string;
            switch (header)
            {
                case Maker.HEADER:
                    newField = new Maker(string.Format("새로운 {0}", Maker.HEADER));
                    break;
                case Measure.HEADER:
                    newField = new Measure(string.Format("새로운 {0}", Measure.HEADER));
                    break;
                case Employee.HEADER:
                    newField = new Employee(string.Format("새로운 {0}", Employee.HEADER));
                    break;
                case Customer.HEADER:
                    newField = new Customer(string.Format("새로운 {0}", Customer.HEADER));
                    break;
                case Supplier.HEADER:
                    newField = new Supplier(string.Format("새로운 {0}", Supplier.HEADER));
                    break;
                case Warehouse.HEADER:
                    newField = new Warehouse(string.Format("새로운 {0}", Warehouse.HEADER));
                    break;
                case Project.HEADER:
                    newField = new Project(string.Format("새로운 {0}", Project.HEADER));
                    break;
                default:
                    throw new NotSupportedException();
            }
            AddField(newField);
        }

        public void AddField(IField field)
        {
            DataDirector.GetInstance().AddField(field);
        }

        public void RemoveField(IObservableField field)
        {
            DataDirector.GetInstance().RemoveField(field);
        }

        public void UpdateNewItem(object item)
        {
            if (item.GetType() == typeof(Observable<Maker>))
                MakerList.Add(item as Observable<Maker>);
            else if (item.GetType() == typeof(Observable<Measure>))
                MeasureList.Add(item as Observable<Measure>);
            else if (item.GetType() == typeof(Observable<Warehouse>))
                WarehouseList.Add(item as Observable<Warehouse>);
            else if (item.GetType() == typeof(Observable<Project>))
                ProjectList.Add(item as Observable<Project>);
            else if (item.GetType() == typeof(Observable<Supplier>))
                SupplierList.Add(item as Observable<Supplier>);
            else if (item.GetType() == typeof(Observable<Customer>))
                CustomerList.Add(item as Observable<Customer>);
            else if (item.GetType() == typeof(Observable<Employee>))
                EmployeeList.Add(item as Observable<Employee>);
        }

        public void UpdateDelItem(object item)
        {
            if (item.GetType() == typeof(Observable<Maker>))
                MakerList.Remove(item as Observable<Maker>);
            else if (item.GetType() == typeof(Observable<Measure>))
                MeasureList.Remove(item as Observable<Measure>);
            else if (item.GetType() == typeof(Observable<Warehouse>))
                WarehouseList.Remove(item as Observable<Warehouse>);
            else if (item.GetType() == typeof(Observable<Project>))
                ProjectList.Remove(item as Observable<Project>);
            else if (item.GetType() == typeof(Observable<Supplier>))
                SupplierList.Remove(item as Observable<Supplier>);
            else if (item.GetType() == typeof(Observable<Customer>))
                CustomerList.Remove(item as Observable<Customer>);
            else if (item.GetType() == typeof(Observable<Employee>))
                EmployeeList.Remove(item as Observable<Employee>);
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));

            DeleteItemCommand.RaiseCanExecuteChanged();
            AddNewItemCommand.RaiseCanExecuteChanged();
        }
    }
}