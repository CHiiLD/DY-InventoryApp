using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    public class ObservableFieldDirector : IObservableFieldDirector
    {
        private static ObservableFieldDirector _thiz;

        private IDictionary<Type, Dictionary<string, IObservableField>> _dictionary;

        private ObservableFieldDirector()
        {
        }

        internal async Task LoadDataFromServerAsync()
        {
            _dictionary = new Dictionary<Type, Dictionary<string, IObservableField>>();

            var customer = await DbAdapter.GetInstance().SelectAllAsync<Customer>();
            var employee = await DbAdapter.GetInstance().SelectAllAsync<Employee>();
            var maker = await DbAdapter.GetInstance().SelectAllAsync<Maker>();
            var measure = await DbAdapter.GetInstance().SelectAllAsync<Measure>();
            var product = await DbAdapter.GetInstance().SelectAllAsync<Product>();
            var project = await DbAdapter.GetInstance().SelectAllAsync<Project>();
            var supplier = await DbAdapter.GetInstance().SelectAllAsync<Supplier>();
            var warehouse = await DbAdapter.GetInstance().SelectAllAsync<Warehouse>();

            _dictionary[typeof(Customer)] = customer.ToDictionary<Customer, string, IObservableField>(x => x.ID, x => new Observable<Customer>(x));
            _dictionary[typeof(Employee)] = employee.ToDictionary<Employee, string, IObservableField>(x => x.ID, x => new Observable<Employee>(x));
            _dictionary[typeof(Maker)] = maker.ToDictionary<Maker, string, IObservableField>(x => x.ID, x => new Observable<Maker>(x));
            _dictionary[typeof(Measure)] = measure.ToDictionary<Measure, string, IObservableField>(x => x.ID, x => new Observable<Measure>(x));
            _dictionary[typeof(Product)] = product.ToDictionary<Product, string, IObservableField>(x => x.ID, x => new Observable<Product>(x));
            _dictionary[typeof(Project)] = project.ToDictionary<Project, string, IObservableField>(x => x.ID, x => new Observable<Project>(x));
            _dictionary[typeof(Supplier)] = supplier.ToDictionary<Supplier, string, IObservableField>(x => x.ID, x => new Observable<Supplier>(x));
            _dictionary[typeof(Warehouse)] = warehouse.ToDictionary<Warehouse, string, IObservableField>(x => x.ID, x => new Observable<Warehouse>(x));
        }

        public static ObservableFieldDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new ObservableFieldDirector();
            return _thiz;
        }

        public static void Destory()
        {
            if (_thiz != null)
                _thiz._dictionary = null;
            _thiz = null;
        }

        public Observable<T> SearchObservableField<T>(string id) where T : class, IField, new()
        {
            if (string.IsNullOrEmpty(id))
                return null;
            Type type = typeof(T);
            return _dictionary[type].ContainsKey(id) ? _dictionary[type][id] as Observable<T> : null;
        }

        public IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new()
        {
            Type type = typeof(T);
            return _dictionary[type].Values.Cast<Observable<T>>();
        }

        public void AddObservableField<T>(IObservableField observableField) where T : class, IField, new()
        {
            if (observableField is IObservableField)
            {
                Type type = typeof(T);
                IObservableField obserableField = observableField as IObservableField;
                if (!_dictionary[type].ContainsKey(obserableField.ID))
                    _dictionary[type].Add(obserableField.ID, obserableField);
            }
        }

        public void RemoveObservableField<T>(IObservableField observableField) where T : class, IField, new()
        {
            Type type = typeof(T);
            if (_dictionary[type].ContainsKey(observableField.ID))
                _dictionary[type].Remove(observableField.ID);
        }
    }
}