using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    internal class ObservableFieldManager
    {
        private IDictionary<Type, Dictionary<string, IObservableField>> _fields;

        internal ObservableFieldManager()
        {
        }

        internal async Task InitializeAsync(IDbAction db)
        {
            _fields = new Dictionary<Type, Dictionary<string, IObservableField>>();

            List<Customer> customer = await db.SelectAsync<Customer>();
            List<Employee> employee = await db.SelectAsync<Employee>();
            List<Maker> maker = await db.SelectAsync<Maker>();
            List<Measure> measure = await db.SelectAsync<Measure>();
            List<Product> product = await db.SelectAsync<Product>();
            List<Project> project = await db.SelectAsync<Project>();
            List<Supplier> supplier = await db.SelectAsync<Supplier>();
            List<Warehouse> warehouse = await db.SelectAsync<Warehouse>();

            _fields[typeof(Customer)] = customer.ToDictionary<Customer, string, IObservableField>(x => x.ID, x => new Observable<Customer>(x));
            _fields[typeof(Employee)] = employee.ToDictionary<Employee, string, IObservableField>(x => x.ID, x => new Observable<Employee>(x));
            _fields[typeof(Maker)] = maker.ToDictionary<Maker, string, IObservableField>(x => x.ID, x => new Observable<Maker>(x));
            _fields[typeof(Measure)] = measure.ToDictionary<Measure, string, IObservableField>(x => x.ID, x => new Observable<Measure>(x));
            _fields[typeof(Product)] = product.ToDictionary<Product, string, IObservableField>(x => x.ID, x => new Observable<Product>(x));
            _fields[typeof(Project)] = project.ToDictionary<Project, string, IObservableField>(x => x.ID, x => new Observable<Project>(x));
            _fields[typeof(Supplier)] = supplier.ToDictionary<Supplier, string, IObservableField>(x => x.ID, x => new Observable<Supplier>(x));
            _fields[typeof(Warehouse)] = warehouse.ToDictionary<Warehouse, string, IObservableField>(x => x.ID, x => new Observable<Warehouse>(x));
        }

        public Observable<T> Search<T>(string id) where T : class, IField, new()
        {
            if (string.IsNullOrEmpty(id))
                return null;
            Type type = typeof(T);
            return _fields[type].ContainsKey(id) ? _fields[type][id] as Observable<T> : null;
        }

        public IEnumerable<Observable<T>> Copy<T>() where T : class, IField, new()
        {
            Type type = typeof(T);
            return _fields[type].Values.Cast<Observable<T>>();
        }

        public void Add<T>(IObservableField observableField) where T : class, IField, new()
        {
            Type type = typeof(T);
            IObservableField obserableField = observableField as IObservableField;
            if (!_fields[type].ContainsKey(obserableField.ID))
                _fields[type].Add(obserableField.ID, obserableField);
        }

        public void Add(IObservableField observableField)
        {
            Type type = observableField.Field.GetType();
            IObservableField obserableField = observableField as IObservableField;
            if (!_fields[type].ContainsKey(obserableField.ID))
                _fields[type].Add(obserableField.ID, obserableField);
        }

        public void Delete<T>(string id) where T : class, IField, new()
        {
            Type type = typeof(T);
            if (_fields[type].ContainsKey(id))
                _fields[type].Remove(id);
        }

        public void Delete(IObservableField observableField)
        {
            Type type = observableField.Field.GetType();
            if (_fields[type].ContainsKey(observableField.ID))
                _fields[type].Remove(observableField.ID);
        }
    }
}