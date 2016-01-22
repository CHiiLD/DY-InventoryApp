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
        private DbAdapter _db;

        private InventoryDataCommander()
        {
            _field = ObservableFieldDirector.GetInstance();
            _inventory = ObservableInventoryDirector.GetInstance();
            _subject = CollectionViewModelObserverSubject.GetInstance();
            _db = DbAdapter.GetInstance();
        }

        ~InventoryDataCommander()
        {
        }

        public static InventoryDataCommander GetInstance()
        {
            if (_me == null)
                _me = new InventoryDataCommander();
            return _me;
        }

        public static void Destroy()
        {
            if (_me != null)
                _me = null;
        }

        #region inventory director

        /// <summary>
        /// 새로운 인벤토리 포멧 데이터를 등록한다.
        /// </summary>
        /// <param name="observableInventory"></param>
        /// <returns></returns>
        public async Task AddObservableInventory(ObservableInventory observableInventory)
        {
            await _db.InsertAsync(observableInventory.Format);
            _inventory.AddObservableInventory(observableInventory);
            _subject.NotifyNewItemAdded(observableInventory);
        }

        public List<ObservableInventory> CopyObservableInventories()
        {
            return _inventory.CopyObservableInventories();
        }

        /// <summary>
        /// 기존의 인벤토리 포맷 데이터를 삭제한다.
        /// </summary>
        /// <param name="observableInventory"></param>
        /// <returns></returns>
        public async Task<bool> RemoveObservableInventory(ObservableInventory observableInventory)
        {
            await _db.DeleteAsync(observableInventory.Format);
            _subject.NotifyItemDeleted(observableInventory);
            return _inventory.RemoveObservableInventory(observableInventory);
        }

        public ObservableInventory SearchObservableInventory(string id)
        {
            return _inventory.SearchObservableInventory(id);
        }

        public IEnumerable<ObservableInventory> SearchObservableInventoryAsProductID(string id)
        {
            return _inventory.SearchObservableInventoryAsProductID(id);
        }

        #endregion inventory director

        #region field director

        public IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new()
        {
            return _field.CopyObservableFields<T>();
        }

        public Observable<T> SearchObservableField<T>(string id) where T : class, IField, new()
        {
            return _field.SearchObservableField<T>(id);
        }

        /// <summary>
        /// 새로운 필드 데이터를 등록한다.
        /// </summary>
        /// <param name="observableField"></param>
        /// <returns></returns>
        public async Task AddObservableField(IObservableField observableField)
        {
            IField field = observableField.Field;
            Type type = field.GetType();
            if (type == typeof(Product))
                await _db.InsertAsync<Product>(field as Product);
            else if (type == typeof(Maker))
                await _db.InsertAsync<Maker>(field as Maker);
            else if (type == typeof(Measure))
                await _db.InsertAsync<Measure>(field as Measure);
            else if (type == typeof(Customer))
                await _db.InsertAsync<Customer>(field as Customer);
            else if (type == typeof(Supplier))
                await _db.InsertAsync<Supplier>(field as Supplier);
            else if (type == typeof(Project))
                await _db.InsertAsync<Project>(field as Project);
            else if (type == typeof(Warehouse))
                await _db.InsertAsync<Warehouse>(field as Warehouse);
            else if (type == typeof(Employee))
                await _db.InsertAsync<Employee>(field as Employee);

            _field.AddObservableField(observableField);
            _subject.NotifyNewItemAdded(observableField);
        }

        /// <summary>
        /// 기존의 필드 데이터를 삭제한다.
        /// </summary>
        /// <param name="observableField"></param>
        /// <returns></returns>
        public async Task RemoveObservableField(IObservableField observableField)
        {
            var field = observableField.Field;
            Type type = field.GetType();
            if (type == typeof(Product))
            {
                List<ObservableInventory> inventories = SearchObservableInventoryAsProductID(observableField.ID).ToList();
                inventories.ForEach(x => _inventory.RemoveObservableInventory(x));
                _field.RemoveObservableField(observableField);
                await DbAdapter.GetInstance().DeleteAsync(observableField.Field as Product);
            }
            else if (type == typeof(Maker))
            {
                await _db.DeleteAsync(field as Maker);
                CopyObservableInventories().ForEach(x =>
                {
                    if (x.Maker != null && x.Maker.ID == field.ID)
                        x.Maker = null;
                });
            }
            else if (type == typeof(Measure))
            {
                await _db.DeleteAsync(field as Measure);
                CopyObservableInventories().ForEach(x =>
                {
                    if (x.Measure != null && x.Measure.ID == field.ID)
                        x.Measure = null;
                });
            }
            else if (type == typeof(Customer))
                await _db.DeleteAsync(field as Customer);
            else if (type == typeof(Supplier))
                await _db.DeleteAsync(field as Supplier);
            else if (type == typeof(Project))
                await _db.DeleteAsync(field as Project);
            else if (type == typeof(Warehouse))
                await _db.DeleteAsync(field as Warehouse);
            else if (type == typeof(Employee))
                await _db.DeleteAsync(field as Employee);

            _field.RemoveObservableField(observableField);
            _subject.NotifyItemDeleted(observableField);
        }

        #endregion field director
    }
}