using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryPipeCollectionDirector : IDirectorAction
    {
        static InventoryPipeCollectionDirector _thiz;
        ObservableCollection<InventoryWrapper> _collection;

        InventoryPipeCollectionDirector()
        {
            _collection = new ObservableCollection<InventoryWrapper>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var invens = db.LoadAll<Inventory>();
                foreach (var item in invens)
                    _collection.Add(new InventoryWrapper(item));
            }
        }

        public static InventoryPipeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new InventoryPipeCollectionDirector();
            return _thiz;
        }

        public void Add(object newPipe)
        {
            if (!_collection.Contains(newPipe))
                _collection.Add(newPipe as InventoryWrapper);
        }

        public void Remove(object pipe)
        {
            if (_collection.Contains(pipe))
                _collection.Remove(pipe as InventoryWrapper);
        }

        public ObservableCollection<InventoryWrapper> LoadPipe()
        {
            return _collection;
        }
    }
}