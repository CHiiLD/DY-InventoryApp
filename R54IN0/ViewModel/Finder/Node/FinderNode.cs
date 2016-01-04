using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace R54IN0
{
    public class FinderNode : INotifyPropertyChanged
    {
        private NodeType _type;
        private string _name;
        private bool _isInEditMode;
        private string _itemID;

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

        public string ItemID
        {
            get
            {
                return _itemID;
            }
            set
            {
                _itemID = value;
                if (!string.IsNullOrEmpty(_itemID) && Type == NodeType.PRODUCT)
                {
                    var itemws = FieldWrapperDirector.GetInstance().CreateCollection<Item, ItemWrapper>();
                    var itemw = itemws.Where(x => x.ID == _itemID).SingleOrDefault();
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
                    case NodeType.FORDER:
                        return !IsInEditMode;

                    case NodeType.PRODUCT:
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
                    case NodeType.FORDER:
                        return true;

                    case NodeType.PRODUCT:
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
                    case NodeType.FORDER:
                        return true;

                    case NodeType.PRODUCT:
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
                    case NodeType.FORDER:
                        return Brushes.Tan;

                    case NodeType.PRODUCT:
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
                    case NodeType.FORDER:
                        return _isInEditMode;

                    case NodeType.PRODUCT:
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

        public string ID
        {
            get;
            set;
        }

        public FinderNode()
        {
            ID = Guid.NewGuid().ToString();
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
            ID = thiz.ID;
            ItemID = thiz.ItemID;
        }

        private void OnItemWrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && Type == NodeType.PRODUCT)
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