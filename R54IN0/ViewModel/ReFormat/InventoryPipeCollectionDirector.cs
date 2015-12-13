using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class InventoryPipeCollectionDirector
    {
        static InventoryPipeCollectionDirector _thiz;
        ObservableCollection<InventoryPipe> _collection;

        InventoryPipeCollectionDirector()
        {
            _collection = new ObservableCollection<InventoryPipe>();
            using (var db = DatabaseDirector.GetDbInstance())
            {
                var invens = db.LoadAll<Inventory>();
                foreach (var item in invens)
                    _collection.Add(new InventoryPipe(item));
            }
        }

        public static InventoryPipeCollectionDirector GetInstance()
        {
            if (_thiz == null)
                _thiz = new InventoryPipeCollectionDirector();
            return _thiz;
        }

        public ObservableCollection<InventoryPipe> LoadPipe()
        {
            return _collection;
        }
    }
}