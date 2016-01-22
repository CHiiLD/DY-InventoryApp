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
    public class DataCommander : IObservableInventoryDirector, IObservableFieldDirector
    {
        private static DataCommander _me;
        private ObservableFieldDirector _fieldDir;
        private ObservableInventoryDirector _inventoryDir;
        private CollectionViewModelObserverSubject _subject;

        private DataCommander()
        {
            _fieldDir = ObservableFieldDirector.GetInstance();
            _inventoryDir = ObservableInventoryDirector.GetInstance();
            _subject = CollectionViewModelObserverSubject.GetInstance();
        }

        ~DataCommander()
        {
            ObservableFieldDirector.Destory();
            ObservableInventoryDirector.Destory();
        }

        public static DataCommander GetInstance()
        {
            if (_me == null)
                _me = new DataCommander();
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

        public void AddObservableField<T>(IObservableField observableField) where T : class, IField, new()
        {
            _fieldDir.AddObservableField<T>(observableField);
            _subject.NotifyNewItemAdded(observableField);
        }

        public IEnumerable<Observable<T>> CopyObservableFields<T>() where T : class, IField, new()
        {
            return _fieldDir.CopyObservableFields<T>();
        }

        public void RemoveObservableField<T>(IObservableField observableField) where T : class, IField, new()
        {
            _subject.NotifyItemDeleted(observableField);
            _fieldDir.RemoveObservableField<T>(observableField);
        }

        public Observable<T> SearchObservableField<T>(string id) where T : class, IField, new()
        {
            return _fieldDir.SearchObservableField<T>(id);
        }
        #endregion
    }
}