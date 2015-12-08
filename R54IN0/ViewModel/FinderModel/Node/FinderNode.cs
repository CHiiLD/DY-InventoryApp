using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace R54IN0
{
    public class FinderNode
    {
        NodeType _type;
        string _name;
        bool _isInEditMode;

        public ObservableCollection<FinderNode> Nodes { get; set; }

        public NodeType Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value == NodeType.NONE)
                    throw new NotSupportedException();
                _type = value;
            }
        }

        public string ItemUUID
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                if (Type == NodeType.ITEM)
                {
                    using (var db = DatabaseDirector.GetDbInstance())
                    {
                        var item = db.LoadByKey<Item>(ItemUUID);
                        return item.Name;
                    }
                }
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public bool AllowDrag
        {
            get
            {
                switch (Type)
                {
                    case NodeType.DIRECTORY:
                        return !IsInEditMode;
                    case NodeType.ITEM:
                        return true;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
            }
        }

        public bool AllowDrop
        {
            get
            {
                switch (Type)
                {
                    case NodeType.DIRECTORY:
                        return true;
                    case NodeType.ITEM:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
            }
        }

        public bool AllowInsert
        {
            get
            {
                switch (Type)
                {
                    case NodeType.DIRECTORY:
                        return true;
                    case NodeType.ITEM:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
            }
        }

        public Brush Color
        {

            get
            {
                switch (Type)
                {
                    case NodeType.DIRECTORY:
                        return Brushes.Tan;
                    case NodeType.ITEM:
                        return Brushes.DeepPink;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
            }
        }

        public bool IsInEditMode
        {
            get
            {
                switch (Type)
                {
                    case NodeType.DIRECTORY:
                        return _isInEditMode;
                    case NodeType.ITEM:
                        return false;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
                _isInEditMode = value;
            }
        }

        public string UUID
        {
            get;
            set;
        }

        public FinderNode()
        {
            UUID = Guid.NewGuid().ToString();
            Nodes = new ObservableCollection<FinderNode>();
        }

        public FinderNode(NodeType type) : this()
        {
            
            if (type == NodeType.NONE)
                throw new ArgumentException();
            Type = type;
        }

        public FinderNode(FinderNode thiz) : this(thiz._type)
        {
            _name = thiz._name;
            UUID = thiz.UUID;
            ItemUUID = thiz.ItemUUID;
        }
    }
}