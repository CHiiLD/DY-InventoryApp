﻿using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    /// <summary>
    /// 데이터 포맷 관리자
    /// </summary>
    public class DataDirector
    {
        private static DataDirector _me;
        private ObservableFieldDirector _field;
        private ObservableInventoryDirector _inventory;
        private CollectionViewModelObserverSubject _subject;
        private SQLiteClient _db;

        private DataDirector()
        {
        }

        ~DataDirector()
        {
        }

        public SQLiteClient DB
        {
            get
            {
                return _db;
            }
        }

        /// <summary>
        /// sqlite에서 가져온 IOStockFormat데이터를 여기 객체에서 관리한다.
        /// </summary>
        public Dictionary<string, ObservableIOStock> ObservableIOStockDictionary
        {
            get;
            set;
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

            _inventory.AddObservableInventory(oInventory);
            _subject.NotifyNewItemAdded(oInventory);

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
            _db.Delete(oInventory.Format);
        }

        public ObservableInventory SearchInventory(string inventoryID)
        {
            return _inventory.SearchObservableInventory(inventoryID);
        }

        public IEnumerable<ObservableInventory> SearchInventories(string productID)
        {
            return _inventory.SearchObservableInventoryAsProductID(productID);
        }

        #endregion inventory director

        #region field director

        /// <summary>
        /// 새로운 필드 데이터를 등록한다.
        /// </summary>
        /// <param name="oField"></param>
        /// <returns></returns>
        public void AddField(IObservableField oField)
        {
            if (oField.Field.ID == null)
                oField.Field.ID = Guid.NewGuid().ToString();

            _field.AddObservableField(oField);
            _subject.NotifyNewItemAdded(oField);

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
        }

        public IEnumerable<Observable<T>> CopyFields<T>() where T : class, IField, new()
        {
            return _field.CopyObservableFields<T>();
        }

        public Observable<T> SearchField<T>(string id) where T : class, IField, new()
        {
            return _field.SearchObservableField<T>(id);
        }

        /// <summary>
        /// 기존의 필드 데이터를 삭제한다.
        /// </summary>
        /// <param name="oFIeld"></param>
        /// <returns></returns>
        public void RemoveField(IObservableField oFIeld)
        {
            IField iField = oFIeld.Field;

            if (iField is Product)
                _db.Delete<Product>(oFIeld.Field);
            else if (iField is Maker)
                _db.Delete<Maker>(iField);
            else if (iField is Measure)
                _db.Delete<Measure>(iField);
            else if (iField is Customer)
                _db.Delete<Customer>(iField);
            else if (iField is Supplier)
                _db.Delete<Supplier>(iField);
            else if (iField is Project)
                _db.Delete<Project>(iField);
            else if (iField is Warehouse)
                _db.Delete<Warehouse>(iField);
            else if (iField is Employee)
                _db.Delete<Employee>(iField);
        }

        #endregion field director

        #region event callback

        private void DataInserted(object obj, SQLInsDelEventArgs e)
        {
            object data = e.Data;
            if (data is InventoryFormat)
            {
                InventoryFormat fmt = data as InventoryFormat;
                ObservableInventory oInventory = new ObservableInventory(fmt);
                if (_inventory.SearchObservableInventory(fmt.ID) == null)
                {
                    _inventory.AddObservableInventory(oInventory);
                    _subject.NotifyNewItemAdded(oInventory);
                }
            }
            else if (data is IOStockFormat)
            {
                IOStockFormat fmt = data as IOStockFormat;
                ObservableIOStock oIOStock = new ObservableIOStock(fmt);
                _subject.NotifyNewItemAdded(oIOStock);
            }
            else if (data is Product)
            {
                Product field = data as Product;
                Observable<Product> oField = new Observable<Product>(field);
                if (_field.SearchObservableField<Product>(field.ID) == null)
                {
                    _field.AddObservableField<Product>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Maker)
            {
                Maker field = data as Maker;
                Observable<Maker> oField = new Observable<Maker>(field);
                if (_field.SearchObservableField<Maker>(field.ID) == null)
                {
                    _field.AddObservableField<Maker>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Measure)
            {
                Measure field = data as Measure;
                Observable<Measure> oField = new Observable<Measure>(field);
                if (_field.SearchObservableField<Measure>(field.ID) == null)
                {
                    _field.AddObservableField<Measure>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Customer)
            {
                Customer field = data as Customer;
                Observable<Customer> oField = new Observable<Customer>(field);
                if (_field.SearchObservableField<Customer>(field.ID) == null)
                {
                    _field.AddObservableField<Customer>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Supplier)
            {
                Supplier field = data as Supplier;
                Observable<Supplier> oField = new Observable<Supplier>(field);
                if (_field.SearchObservableField<Supplier>(field.ID) == null)
                {
                    _field.AddObservableField<Supplier>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Project)
            {
                Project field = data as Project;
                Observable<Project> oField = new Observable<Project>(field);
                if (_field.SearchObservableField<Project>(field.ID) == null)
                {
                    _field.AddObservableField<Project>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Warehouse)
            {
                Warehouse field = data as Warehouse;
                Observable<Warehouse> oField = new Observable<Warehouse>(field);
                if (_field.SearchObservableField<Warehouse>(field.ID) == null)
                {
                    _field.AddObservableField<Warehouse>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
            else if (data is Employee)
            {
                Employee field = data as Employee;
                Observable<Employee> oField = new Observable<Employee>(field);
                if (_field.SearchObservableField<Employee>(field.ID) == null)
                {
                    _field.AddObservableField<Employee>(oField);
                    _subject.NotifyNewItemAdded(oField);
                }
            }
        }

        private void DataUpdated(object obj, SQLUpdateEventArgs e)
        {
            object data = e.Data;

            if (data is InventoryFormat)
            {
                InventoryFormat fmt = data as InventoryFormat;
                ObservableInventory oInventory = SearchInventory(fmt.ID);
                if (oInventory != null)
                    oInventory.Format = fmt;
            }
            else if (data is IOStockFormat)
            {
                IOStockFormat fmt = data as IOStockFormat;
                if (ObservableIOStockDictionary.ContainsKey(fmt.ID))
                {
                    ObservableIOStock oIOStock = ObservableIOStockDictionary[fmt.ID];
                    oIOStock.Format = fmt;
                }
            }
            else if (data is IField)
            {
                IObservableField oField = null;
                IField field = data as IField;
                if (data is Product)
                    oField = SearchField<Product>(field.ID);
                else if (data is Maker)
                    oField = SearchField<Maker>(field.ID);
                else if (data is Measure)
                    oField = SearchField<Measure>(field.ID);
                else if (data is Customer)
                    oField = SearchField<Customer>(field.ID);
                else if (data is Supplier)
                    oField = SearchField<Supplier>(field.ID);
                else if (data is Project)
                    oField = SearchField<Project>(field.ID);
                else if (data is Warehouse)
                    oField = SearchField<Warehouse>(field.ID);
                else if (data is Employee)
                    oField = SearchField<Employee>(field.ID);
                oField.Field = field;
            }
        }

        private void DataDeleted(object obj, SQLInsDelEventArgs e)
        {
            object data = e.Data;
            if (data is InventoryFormat)
            {
                InventoryFormat fmt = data as InventoryFormat;
                ObservableInventory oInventory = SearchInventory(fmt.ID);
                if (oInventory != null)
                {
                    _subject.NotifyItemDeleted(oInventory);
                    _inventory.RemoveObservableInventory(oInventory.ID);
                }
            }
            else if (data is IOStockFormat)
            {
                IOStockFormat fmt = data as IOStockFormat;
                if (ObservableIOStockDictionary.ContainsKey(fmt.ID))
                {
                    _subject.NotifyItemDeleted(ObservableIOStockDictionary[fmt.ID]);
                    ObservableIOStockDictionary.Remove(fmt.ID);
                }
            }
            else if (data is Product)
            {
                Product field = data as Product;
                Observable<Product> oField = SearchField<Product>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Product>(oField.ID);
                }
            }
            else if (data is Maker)
            {
                Maker field = data as Maker;
                Observable<Maker> oField = SearchField<Maker>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Maker>(oField.ID);
                }
            }
            else if (data is Measure)
            {
                Measure field = data as Measure;
                Observable<Measure> oField = SearchField<Measure>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Measure>(oField.ID);
                }
            }
            else if (data is Customer)
            {
                Customer field = data as Customer;
                Observable<Customer> oField = SearchField<Customer>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Customer>(oField.ID);
                }
            }
            else if (data is Supplier)
            {
                Supplier field = data as Supplier;
                Observable<Supplier> oField = SearchField<Supplier>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Supplier>(oField.ID);
                }
            }
            else if (data is Project)
            {
                Project field = data as Project;
                Observable<Project> oField = SearchField<Project>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Project>(oField.ID);
                }
            }
            else if (data is Warehouse)
            {
                Warehouse field = data as Warehouse;
                Observable<Warehouse> oField = SearchField<Warehouse>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Warehouse>(oField.ID);
                }
            }
            else if (data is Employee)
            {
                Employee field = data as Employee;
                Observable<Employee> oField = SearchField<Employee>(field.ID);
                if (oField != null)
                {
                    _subject.NotifyItemDeleted(oField);
                    _field.RemoveObservableField<Employee>(oField.ID);
                }
            }
        }

        #endregion event callback

        private void Initialze()
        {
            _db = new SQLiteClient();
            if (_db.Open())
            {
                _field = new ObservableFieldDirector(_db);
                _inventory = new ObservableInventoryDirector(_db);
                _subject = CollectionViewModelObserverSubject.GetInstance();
#if false
                _db.DataInsertEventHandler += DataInserted;
#endif
                _db.DataUpdateEventHandler += DataUpdated;
                _db.DataDeleteEventHandler += DataDeleted;
            }
        }
    }
}