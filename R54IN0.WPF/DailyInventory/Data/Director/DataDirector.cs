using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    /// <summary>
    /// 데이터 포맷 관리자
    /// </summary>
    public partial class DataDirector
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static DataDirector _me;
        private ObservableFieldManager _field;
        private ObservableInventoryManager _inventory;
        private CollectionViewModelObserverSubject _subject;
        private MySqlBridge _bridge;

        private DataDirector()
        {
            _subject = CollectionViewModelObserverSubject.GetInstance();
            StockList = new List<IOStockDataGridItem>();
        }

        ~DataDirector()
        {
        }

        public MySqlBridge DB
        {
            get
            {
                return _bridge;
            }
        }

        public List<IOStockDataGridItem> StockList
        {
            get;
            private set;
        }

        public static DataDirector GetInstance()
        {
            if (_me == null)
            {
                _me = new DataDirector();
            }
            return _me;
        }

        public static void Destroy()
        {
            if (_me != null)
            {
                _me._field = null;
                _me._inventory = null;
                _me._subject = null;
                _me._bridge.Dispose();
                _me._bridge = null;
                _me = null;
            }
        }
        #region inventory director

        /// <summary>
        /// 새로운 인벤토리 포멧 데이터를 등록한다.
        /// </summary>
        /// <param name="inventoryFormat"></param>
        /// <returns></returns>
        public void AddInventory(InventoryFormat invf)
        {
            if (invf.ID == null)
                invf.ID = Guid.NewGuid().ToString();
            _bridge.Insert(invf);
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
            _bridge.Delete<InventoryFormat>(oInventory.ID);
        }

        public ObservableInventory SearchInventory(string inventoryID)
        {
            return _inventory.Search(inventoryID);
        }

        public List<ObservableInventory> SearchInventories(string productID)
        {
            return _inventory.SearchAsProductID(productID);
        }
        #endregion inventory director

        #region field director
        public void AddField(IField field)
        {
            if (field == null)
                throw new ArgumentNullException();

            if (field is Product)
                _bridge.Insert<Product>(field);
            else if (field is Maker)
                _bridge.Insert<Maker>(field);
            else if (field is Measure)
                _bridge.Insert<Measure>(field);
            else if (field is Customer)
                _bridge.Insert<Customer>(field);
            else if (field is Supplier)
                _bridge.Insert<Supplier>(field);
            else if (field is Project)
                _bridge.Insert<Project>(field);
            else if (field is Warehouse)
                _bridge.Insert<Warehouse>(field);
            else if (field is Employee)
                _bridge.Insert<Employee>(field);
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
        /// <param name="ofield"></param>
        /// <returns></returns>
        public void RemoveField(IObservableField ofield)
        {
            IField ifield = ofield.Field;
            string id = ofield.ID;

            if (ifield is Product)
                _bridge.Delete<Product>(id);
            else if (ifield is Maker)
                _bridge.Delete<Maker>(id);
            else if (ifield is Measure)
                _bridge.Delete<Measure>(id);
            else if (ifield is Customer)
                _bridge.Delete<Customer>(id);
            else if (ifield is Supplier)
                _bridge.Delete<Supplier>(id);
            else if (ifield is Project)
                _bridge.Delete<Project>(id);
            else if (ifield is Warehouse)
                _bridge.Delete<Warehouse>(id);
            else if (ifield is Employee)
                _bridge.Delete<Employee>(id);
            else
                throw new NotSupportedException();
        }
        #endregion field director

        public static async Task InstanceInitialzeAsync()
        {
            DataDirector ddr = GetInstance();
            MySqlBridge bridge = ddr._bridge = new MySqlBridge();

            bridge.DataInsertEventHandler += ddr.OnDataInserted;
            bridge.DataUpdateEventHandler += ddr.OnDataUpdated;
            bridge.DataDeleteEventHandler += ddr.OnDataDeleted;
            //TODO 동기화 문제 
            bridge.Connect();

            await Task.Delay(2000);

            ddr.StockList = new List<IOStockDataGridItem>();
            ddr._field = new ObservableFieldManager(bridge);
            ddr._inventory = new ObservableInventoryManager(bridge);
            await ddr._field.InitializeAsync();
            await ddr._inventory.InitializeAsync();
        }
    }
}