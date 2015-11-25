using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media;

namespace R54IN0
{
    public class ItemNode : DirectoryNode
    {
        string _itemUUID;

        public override string Name
        {
            get
            {
                var item = DatabaseDirector.GetDbInstance().LoadByKey<Item>(_itemUUID);
                Debug.Assert(item != null);
                return item.Name;
            }
        }

        public override bool AllowDrop
        {
            get
            {
                return false;
            }
        }

        public override bool AllowInsert
        {
            get
            {
                return false;
            }
        }

        public override Brush Color
        {
            get
            {
                return Brushes.DeepPink;
            }
        }

        public override bool IsEditable
        {
            get
            {
                return false;
            }
        }

        public bool IsDelete
        {
            get
            {
                var item = DatabaseDirector.GetDbInstance().LoadByKey<Item>(_itemUUID);
                Debug.Assert(item != null);
                return item.IsDeleted;
            }
        }

        public string ItemUUID
        {
            get
            {
                return _itemUUID;
            }
        }

        public ItemNode(ItemNode thiz) : base(thiz)
        {
            _itemUUID = thiz._itemUUID;
        }

        public ItemNode() : base()
        {
        }

        public ItemNode(string itemUUID) : this()
        {
            _itemUUID = itemUUID;
        }
    }
}