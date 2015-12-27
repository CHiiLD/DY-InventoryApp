using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;

namespace R54IN0
{
    public class FinderNode : INotifyPropertyChanged
    {
        NodeType _type;
        string _name;
        bool _isInEditMode;
        string _itemUuid;

        public event PropertyChangedEventHandler PropertyChanged;

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
            get
            {
                return _itemUuid;
            }
            set
            {
                _itemUuid = value;
                if (!string.IsNullOrEmpty(_itemUuid) && Type == NodeType.ITEM)
                {
                    var itemws = FieldWrapperDirector.GetInstance().CreateCollection<Item, ItemWrapper>();
                    var itemw = itemws.Where(x => x.UUID == _itemUuid).SingleOrDefault();
                    if (itemw != null)
                    {
                        Name = itemw.Name;
                        itemw.PropertyChanged += OnItemWrapperPropertyChanged;
                    }
                }
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
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

        void OnItemWrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Name" && Type == NodeType.ITEM)
            {
                var itemw = sender as ItemWrapper;
                Name = itemw.Name;
            }
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}