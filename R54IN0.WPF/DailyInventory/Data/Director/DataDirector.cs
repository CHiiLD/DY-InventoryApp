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
        private IDbAction _dbAction;

        private DataDirector()
        {
            StockList = new List<IOStockDataGridItem>();
            _subject = CollectionViewModelObserverSubject.GetInstance();
            _field = new ObservableFieldManager();
            _inventory = new ObservableInventoryManager();
        }

        ~DataDirector()
        {
        }

        public IDbAction Db
        {
            get
            {
                return _dbAction;
            }
            set
            {
                _dbAction = value;
                _dbAction.DataInsertEventHandler += OnDataInserted;
                _dbAction.DataUpdateEventHandler += OnDataUpdated;
                _dbAction.DataDeleteEventHandler += OnDataDeleted;

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
                _me._dbAction.Dispose();
                _me._dbAction = null;
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
            _dbAction.Insert(invf);
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
            _dbAction.Delete<InventoryFormat>(oInventory.ID);
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
                _dbAction.Insert<Product>(field);
            else if (field is Maker)
                _dbAction.Insert<Maker>(field);
            else if (field is Measure)
                _dbAction.Insert<Measure>(field);
            else if (field is Customer)
                _dbAction.Insert<Customer>(field);
            else if (field is Supplier)
                _dbAction.Insert<Supplier>(field);
            else if (field is Project)
                _dbAction.Insert<Project>(field);
            else if (field is Warehouse)
                _dbAction.Insert<Warehouse>(field);
            else if (field is Employee)
                _dbAction.Insert<Employee>(field);
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
                _dbAction.Delete<Product>(id);
            else if (ifield is Maker)
                _dbAction.Delete<Maker>(id);
            else if (ifield is Measure)
                _dbAction.Delete<Measure>(id);
            else if (ifield is Customer)
                _dbAction.Delete<Customer>(id);
            else if (ifield is Supplier)
                _dbAction.Delete<Supplier>(id);
            else if (ifield is Project)
                _dbAction.Delete<Project>(id);
            else if (ifield is Warehouse)
                _dbAction.Delete<Warehouse>(id);
            else if (ifield is Employee)
                _dbAction.Delete<Employee>(id);
            else
                throw new NotSupportedException();
        }
        #endregion field director

        public static async Task InitialzeInstanceAsync(int connectionTimeout = 1000)
        {
            Destroy();
            DataDirector ddr = GetInstance();
            MySqlBridge bridge = new MySqlBridge();
            ddr.Db = bridge;
            IAsyncResult ar = bridge.Connect();
            ar.AsyncWaitHandle.WaitOne(connectionTimeout);

            if (bridge.Socket.Connected)
            {
                await ddr._field.InitializeAsync(bridge);
                await ddr._inventory.InitializeAsync(bridge);
            }
        }

        public static void IntializeInstance(IDbAction dbAction)
        {
            Destroy();
            DataDirector ddr = GetInstance();
            ddr.Db = dbAction;
            ddr._field.InitializeAsync(dbAction).Wait();
            ddr._inventory.InitializeAsync(dbAction).Wait();
        }
    }
}