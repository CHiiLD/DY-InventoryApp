using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0
{
    internal class ObservableFieldDirector
    {
        private IDictionary<Type, Dictionary<string, IObservableField>> _dictionary;
        private SQLiteServer _db;

        internal ObservableFieldDirector(SQLiteServer _db)
        {
            this._db = _db;
            Load();
        }

        internal void Load()
        {
            _dictionary = new Dictionary<Type, Dictionary<string, IObservableField>>();

            var customer = _db.Select<Customer>();
            var employee = _db.Select<Employee>();
            var maker = _db.Select<Maker>();
            var measure = _db.Select<Measure>();
            var product = _db.Select<Product>();
            var project = _db.Select<Project>();
            var supplier = _db.Select<Supplier>();
            var warehouse = _db.Select<Warehouse>();

            _dictionary[typeof(Customer)] = customer.ToDictionary<Customer, string, IObservableField>(x => x.ID, x => new Observable<Customer>(x));
            _dictionary[typeof(Employee)] = employee.ToDictionary<Employee, string, IObservableField>(x => x.ID, x => new Observable<Employee>(x));
            _dictionary[typeof(Maker)] = maker.ToDictionary<Maker, string, IObservableField>(x => x.ID, x => new Observable<Maker>(x));
            _dictionary[typeof(Measure)] = measure.ToDictionary<Measure, string, IObservableField>(x => x.ID, x => new Observable<Measure>(x));
            _dictionary[typeof(Product)] = product.ToDictionary<Product, string, IObservableField>(x => x.ID, x => new Observable<Product>(x));
            _dictionary[typeof(Project)] = project.ToDictionary<Project, string, IObservableField>(x => x.ID, x => new Observable<Project>(x));
            _dictionary[typeof(Supplier)] = supplier.ToDictionary<Supplier, string, IObservableField>(x => x.ID, x => new Observable<Supplier>(x));
            _dictionary[typeof(Warehouse)] = warehouse.ToDictionary<Warehouse, string, IObservableField>(x => x.ID, x => new Observable<Warehouse>(x));
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
            Type type = typeof(T);
            IObservableField obserableField = observableField as IObservableField;
            if (!_dictionary[type].ContainsKey(obserableField.ID))
                _dictionary[type].Add(obserableField.ID, obserableField);
        }

        public void AddObservableField(IObservableField observableField)
        {
            Type type = observableField.Field.GetType();
            IObservableField obserableField = observableField as IObservableField;
            if (!_dictionary[type].ContainsKey(obserableField.ID))
                _dictionary[type].Add(obserableField.ID, obserableField);
        }

        public void RemoveObservableField<T>(IObservableField observableField) where T : class, IField, new()
        {
            Type type = typeof(T);
            if (_dictionary[type].ContainsKey(observableField.ID))
                _dictionary[type].Remove(observableField.ID);
        }

        public void RemoveObservableField(IObservableField observableField)
        {
            Type type = observableField.Field.GetType();
            if (_dictionary[type].ContainsKey(observableField.ID))
                _dictionary[type].Remove(observableField.ID);
        }
    }
}