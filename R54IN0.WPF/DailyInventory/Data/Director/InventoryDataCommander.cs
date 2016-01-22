using R54IN0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    /// <summary>
    /// 데이터 포맷 관리자
    /// </summary>
    public class InventoryDataCommander
    {
        private static InventoryDataCommander _me;
        private ObservableFieldDirector _fieldDir;
        private ObservableInventoryDirector _inventoryDir;
        private CollectionViewModelObserverSubject _subject;
        private DbAdapter _db;

        private InventoryDataCommander()
        {
            _fieldDir = ObservableFieldDirector.GetInstance();
            _inventoryDir = ObservableInventoryDirector.GetInstance();
            _subject = CollectionViewModelObserverSubject.GetInstance();
            _db = DbAdapter.GetInstance();
        }

        ~InventoryDataCommander()
        {
            ObservableFieldDirector.Destory();
            ObservableInventoryDirector.Destory();
        }

        public static InventoryDataCommander GetInstance()
        {
            if (_me == null)
                _me = new InventoryDataCommander();
            return _me;
        }

        #region inventory director

        public void AddObservableInventory(ObservableInventory observableInventory)
        {
            _subject.NotifyNewItemAdded(observableInventory);
            _inventoryDir.AddObservableInventory(observableInventory);
        }

        public List<ObservableInventory> CopyObservableInventories()
        {
            return _inventoryDir.CopyObservableInventories();
        }

        public bool RemoveObservableInventory(ObservableInventory observableInventory)
        {
            _subject.NotifyItemDeleted(observableInventory);
            return _inventoryDir.RemoveObservableInventory(observableInventory);
        }

        public ObservableInventory SearchObservableInventory(string id)
        {
            return _inventoryDir.SearchObservableInventory(id);
        }

        public IEnumerable<ObservableInventory> SearchObservableInventoryAsProductID(string id)
        {
            return _inventoryDir.SearchObservableInventoryAsProductID(id);
        }
        #endregion

        #region field director 

        public IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new()
        {
            return _fieldDir.CopyObservableFields<T>();
        }

        public Observable<T> SearchObservableField<T>(string id) where T : class, IField, new()
        {
            return _fieldDir.SearchObservableField<T>(id);
        }

        public async Task AddObservableField(IObservableField observableField)
        {
            IField field = observableField.Field;
            Type type = field.GetType();

            if (type == typeof(Maker))
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

            _fieldDir.AddObservableField(observableField);
            _subject.NotifyNewItemAdded(observableField);
        }

        public async Task RemoveObservableField(IObservableField observableField)
        {
            var field = observableField.Field;
            Type type = field.GetType();

            if (type == typeof(Maker))
            {
                await _db.DeleteAsync(field as Maker);
                _inventoryDir.CopyObservableInventories().ForEach(x =>
                {
                    if (x.Maker != null && x.Maker.ID == field.ID)
                        x.Maker = null;
                });
            }
            else if (type == typeof(Measure))
            {
                await _db.DeleteAsync(field as Measure);
                _inventoryDir.CopyObservableInventories().ForEach(x =>
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

            _fieldDir.RemoveObservableField(observableField);
            _subject.NotifyItemDeleted(observableField);
        }
        #endregion
    }
}