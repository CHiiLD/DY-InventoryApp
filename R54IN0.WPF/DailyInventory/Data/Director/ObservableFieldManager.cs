using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    internal class ObservableFieldManager
    {
        private IDictionary<Type, Dictionary<string, IObservableField>> _fields;
        private SQLiteClient _db;

        internal ObservableFieldManager(SQLiteClient _db)
        {
            this._db = _db;
            Load();
        }

        internal void Load()
        {
            _fields = new Dictionary<Type, Dictionary<string, IObservableField>>();

            var customer = _db.Select<Customer>();
            var employee = _db.Select<Employee>();
            var maker = _db.Select<Maker>();
            var measure = _db.Select<Measure>();
            var product = _db.Select<Product>();
            var project = _db.Select<Project>();
            var supplier = _db.Select<Supplier>();
            var warehouse = _db.Select<Warehouse>();

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