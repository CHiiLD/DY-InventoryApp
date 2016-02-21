using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    /// <summary>
    /// 데이터 포맷 관리자
    /// </summary>
    public partial class DataDirector
    {
        private static DataDirector _me;
        private ObservableFieldManager _field;
        private ObservableInventoryManager _inventory;
        private CollectionViewModelObserverSubject _subject;
        private MySQLClient _db;

        private DataDirector()
        {
        }

        ~DataDirector()
        {
        }

        public MySQLClient DB
        {
            get
            {
                return _db;
            }
        }

        public List<IOStockDataGridItem> StockCollection
        {
            get;
            private set;
        }

        public static DataDirector GetInstance()
        {
            if (_me == null)
            {
                _me = new DataDirector();
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
        /// <param name="inventoryFormat"></param>
        /// <returns></returns>
        public void AddInventory(ObservableInventory oInventory)
        {
            if (oInventory.ID == null)
                oInventory.ID = Guid.NewGuid().ToString();

            _db.Insert(oInventory.Format);
        }

        public List<ObservableInventory> CopyInventories()
        {
            return _inventory.CopyObservableInventories();
        }

        /// <summary>
        /// 기존의 인벤토리 포맷 데이터를 삭제한다.
        /// </summary>
        /// <param name="oInventory"></param>
        /// <returns></returns>
        public void RemoveInventory(ObservableInventory oInventory)
        {
            _db.Delete<InventoryFormat>(oInventory.ID);
        }

        public ObservableInventory SearchInventory(string inventoryID)
        {
            return _inventory.Search(inventoryID);
        }

        public IEnumerable<ObservableInventory> SearchInventories(string productID)
        {
            return _inventory.SearchAsProductID(productID);
        }

        #endregion inventory director

        #region field director

#if false
        public void AddField<TableT>(IObservableField oField)
        {
            AddField(oField);
        }

        /// <summary>
        /// 새로운 필드 데이터를 등록한다.
        /// </summary>
        /// <param name="oField"></param>
        /// <returns></returns>
        public void AddField(IObservableField oField)
        {
            if (oField.Field.ID == null)
                oField.Field.ID = Guid.NewGuid().ToString();

            IField iField = oField.Field;

            if (iField is Product)
                _db.Insert<Product>(iField);
            else if (iField is Maker)
                _db.Insert<Maker>(iField);
            else if (iField is Measure)
                _db.Insert<Measure>(iField);
            else if (iField is Customer)
                _db.Insert<Customer>(iField);
            else if (iField is Supplier)
                _db.Insert<Supplier>(iField);
            else if (iField is Project)
                _db.Insert<Project>(iField);
            else if (iField is Warehouse)
                _db.Insert<Warehouse>(iField);
            else if (iField is Employee)
                _db.Insert<Employee>(iField);
            else
                throw new NotSupportedException();
        }
#endif
        public void AddField(IField field)
        {
            if (field == null)
                throw new ArgumentNullException();

            if (field is Product)
                _db.Insert<Product>(field);
            else if (field is Maker)
                _db.Insert<Maker>(field);
            else if (field is Measure)
                _db.Insert<Measure>(field);
            else if (field is Customer)
                _db.Insert<Customer>(field);
            else if (field is Supplier)
                _db.Insert<Supplier>(field);
            else if (field is Project)
                _db.Insert<Project>(field);
            else if (field is Warehouse)
                _db.Insert<Warehouse>(field);
            else if (field is Employee)
                _db.Insert<Employee>(field);
            else
                throw new NotSupportedException();
        }

        public IEnumerable<Observable<T>> CopyFields<T>() where T : class, IField, new()
        {
            return _field.Copy<T>();
        }

        public Observable<T> SearchField<T>(string id) where T : class, IField, new()
        {
            return _field.Search<T>(id);
        }

        /// <summary>
        /// 기존의 필드 데이터를 삭제한다.
        /// </summary>
        /// <param name="oFIeld"></param>
        /// <returns></returns>
        public void RemoveField(IObservableField oFIeld)
        {
            IField iField = oFIeld.Field;
            string id = oFIeld.ID;

            if (iField is Product)
                _db.Delete<Product>(id);
            else if (iField is Maker)
                _db.Delete<Maker>(id);
            else if (iField is Measure)
                _db.Delete<Measure>(id);
            else if (iField is Customer)
                _db.Delete<Customer>(id);
            else if (iField is Supplier)
                _db.Delete<Supplier>(id);
            else if (iField is Project)
                _db.Delete<Project>(id);
            else if (iField is Warehouse)
                _db.Delete<Warehouse>(id);
            else if (iField is Employee)
                _db.Delete<Employee>(id);
            else
                throw new NotSupportedException();
        }

#endregion field director

        private void Initialze()
        {
            _db = new MySQLClient();
            if (_db.Open())
            {
                StockCollection = new List<IOStockDataGridItem>();
                _field = new ObservableFieldManager(_db);
                _inventory = new ObservableInventoryManager(_db);
                _subject = CollectionViewModelObserverSubject.GetInstance();
                _db.DataInsertEventHandler += OnDataInserted;
                _db.DataUpdateEventHandler += OnDataUpdated;
                _db.DataDeleteEventHandler += OnDataDeleted;
            }
        }
    }
}