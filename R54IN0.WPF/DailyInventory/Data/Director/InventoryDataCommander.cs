using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    /// <summary>
    /// 데이터 포맷 관리자
    /// </summary>
    public class InventoryDataCommander
    {
        private static InventoryDataCommander _me;
        private ObservableFieldDirector _field;
        private ObservableInventoryDirector _inventory;
        private CollectionViewModelObserverSubject _subject;
        private SQLiteServer _db;

        private InventoryDataCommander()
        {
            
        }

        ~InventoryDataCommander()
        {
        }

        public SQLiteServer DB
        {
            get
            {
                return _db;
            }
        }

        public static InventoryDataCommander GetInstance()
        {
            if (_me == null)
            {
                _me = new InventoryDataCommander();
                _me.Initialze();
            }
            return _me;
        }

        public static void Destroy()
        {
            if (_me != null)
            {
                _me.DB.Close();
                _me = null;
            }
        }
        #region inventory director

        /// <summary>
        /// 새로운 인벤토리 포멧 데이터를 등록한다.
        /// </summary>
        /// <param name="observableInventory"></param>
        /// <returns></returns>
        public void AddInventory(ObservableInventory observableInventory)
        {
            _db.Insert(observableInventory.Format);
            _inventory.AddObservableInventory(observableInventory);
            _subject.NotifyNewItemAdded(observableInventory);
        }

        public List<ObservableInventory> CopyInventories()
        {
            return _inventory.CopyObservableInventories();
        }

        /// <summary>
        /// 기존의 인벤토리 포맷 데이터를 삭제한다.
        /// </summary>
        /// <param name="observableInventory"></param>
        /// <returns></returns>
        public bool RemoveInventory(ObservableInventory observableInventory)
        {
            _db.Delete(observableInventory.Format);
            _subject.NotifyItemDeleted(observableInventory);
            return _inventory.RemoveObservableInventory(observableInventory);
        }

        public ObservableInventory SearchInventory(string id)
        {
            return _inventory.SearchObservableInventory(id);
        }

        public IEnumerable<ObservableInventory> SearchInventoryAsProductID(string id)
        {
            return _inventory.SearchObservableInventoryAsProductID(id);
        }

        #endregion inventory director

        #region field director

        public IEnumerable<Observable<T>> CopyFields<T>() where T : class, IField, new()
        {
            return _field.CopyObservableFields<T>();
        }

        public Observable<T> SearchField<T>(string id) where T : class, IField, new()
        {
            return _field.SearchObservableField<T>(id);
        }

        /// <summary>
        /// 새로운 필드 데이터를 등록한다.
        /// </summary>
        /// <param name="observableField"></param>
        /// <returns></returns>
        public void AddObservableField(IObservableField observableField)
        {
            IField field = observableField.Field;
            Type type = field.GetType();
            if (type == typeof(Product))
                _db.Insert<Product>(field);
            else if (type == typeof(Maker))
                _db.Insert<Maker>(field);
            else if (type == typeof(Measure))
                _db.Insert<Measure>(field);
            else if (type == typeof(Customer))
                _db.Insert<Customer>(field);
            else if (type == typeof(Supplier))
                _db.Insert<Supplier>(field);
            else if (type == typeof(Project))
                _db.Insert<Project>(field);
            else if (type == typeof(Warehouse))
                _db.Insert<Warehouse>(field);
            else if (type == typeof(Employee))
                _db.Insert<Employee>(field);

            _field.AddObservableField(observableField);
            _subject.NotifyNewItemAdded(observableField);
        }

        /// <summary>
        /// 기존의 필드 데이터를 삭제한다.
        /// </summary>
        /// <param name="observableField"></param>
        /// <returns></returns>
        public void RemoveObservableField(IObservableField observableField)
        {
            var field = observableField.Field;
            Type type = field.GetType();
            if (type == typeof(Product))
            {
                List<ObservableInventory> inventories = SearchInventoryAsProductID(observableField.ID).ToList();
                inventories.ForEach(x => _inventory.RemoveObservableInventory(x));
                _field.RemoveObservableField(observableField);
                _db.Delete<Product>(observableField.Field);
            }
            else if (type == typeof(Maker))
            {
                _db.Delete<Maker>(field);
                CopyInventories().ForEach(x =>
                {
                    if (x.Maker != null && x.Maker.ID == field.ID)
                        x.Maker = null;
                });
            }
            else if (type == typeof(Measure))
            {
                _db.Delete<Measure>(field);
                CopyInventories().ForEach(x =>
                {
                    if (x.Measure != null && x.Measure.ID == field.ID)
                        x.Measure = null;
                });
            }
            else if (type == typeof(Customer))
                _db.Delete<Customer>(field);
            else if (type == typeof(Supplier))
                _db.Delete<Supplier>(field);
            else if (type == typeof(Project))
                _db.Delete<Project>(field);
            else if (type == typeof(Warehouse))
                _db.Delete<Warehouse>(field);
            else if (type == typeof(Employee))
                _db.Delete<Employee>(field);

            _field.RemoveObservableField(observableField);
            _subject.NotifyItemDeleted(observableField);
        }

        #endregion field director

        private void Initialze()
        {
            _db = new SQLiteServer();
            if (_db.Open())
            {
                _field = new ObservableFieldDirector(_db);
                _inventory = new ObservableInventoryDirector(_db);
                _subject = CollectionViewModelObserverSubject.GetInstance();
            }
        }
    }
}