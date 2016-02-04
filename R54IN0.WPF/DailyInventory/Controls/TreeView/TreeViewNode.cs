using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace R54IN0.WPF
{
    public class TreeViewNode : INotifyPropertyChanged
    {
        private NodeType _type;
        private string _name;
        private bool _isNameEditable;
        private string _observableObjectID;

        private event PropertyChangedEventHandler _propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        /// <summary>
        /// json로드를 위한 생성자
        /// </summary>
        public TreeViewNode()
        {
            Root = new ObservableCollection<TreeViewNode>();
        }

        /// <summary>
        /// 폴더 속성의 트리뷰노드를 생성합니다.
        /// </summary>
        /// <param name="folderName"></param>
        public TreeViewNode(string folderName) : this()
        {
            Type = NodeType.FOLDER;
            Name = folderName;
        }

        /// <summary>
        /// 제품 속성의 트리뷰노드를 생성합니다.
        /// </summary>
        /// <param name="product"></param>
        public TreeViewNode(Observable<Product> product) : this()
        {
            if (product == null && string.IsNullOrEmpty(product.ID))
                throw new NotSupportedException();

            Type = NodeType.PRODUCT;
            ObservableObjectID = product.ID;

            var invnetories = InventoryDataCommander.GetInstance().SearchObservableInventoryAsProductID(ObservableObjectID);
            foreach (var inventory in invnetories)
            {
                if (Root.All(x => x.ObservableObjectID != inventory.ID))
                    Root.Add(new TreeViewNode(inventory));
            }
            Root.OrderBy(x => x.Name);
        }

        /// <summary>
        /// 인벤토리 속성의 트리뷰노드를 생성합니다.
        /// </summary>
        /// <param name="inventory"></param>
        public TreeViewNode(ObservableInventory inventory) : this()
        {
            if (inventory == null && string.IsNullOrEmpty(inventory.ID))
                throw new NotSupportedException();

            Type = NodeType.INVENTORY;
            ObservableObjectID = inventory.ID;
        }

        public ObservableCollection<TreeViewNode> Root { get; set; }

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

        public string ObservableObjectID
        {
            get
            {
                return _observableObjectID;
            }
            set
            {
                _observableObjectID = value;
                switch (Type)
                {
                    case NodeType.PRODUCT:
                        Observable<Product> product = GetObservableProduct(value);
                        if (product != null)
                        {
                            Name = product.Name; //이름 적용
                            product.PropertyChanged += OnProductPropertyChanged; //이벤트 적용
                        }
                        break;
                    case NodeType.INVENTORY:
                        ObservableInventory inventory = GetObservableInventory(value);
                        if (inventory != null)
                        {
                            Name = inventory.Specification; //이름 적용
                            inventory.PropertyChanged += OnInventoryPropertyChanged; //이벤트 적용
                        }
                        break;
                    default:
                        throw new NotSupportedException();
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
                NotifyPropertyChanged("Name");
                if (!IsNameEditable)
                    return;
                switch (Type)
                {
                    case NodeType.PRODUCT:
                        Observable<Product> product = GetObservableProduct(ObservableObjectID);
                        if (product != null)
                        {
                            product.PropertyChanged -= OnProductPropertyChanged;
                            product.Name = value;
                            product.PropertyChanged += OnProductPropertyChanged;
                        }
                        break;
                    case NodeType.INVENTORY:
                        ObservableInventory inventory = GetObservableInventory(ObservableObjectID);
                        if (inventory != null)
                        {
                            inventory.PropertyChanged -= OnInventoryPropertyChanged;
                            inventory.Specification = value;
                            inventory.PropertyChanged += OnInventoryPropertyChanged;
                        }
                        break;
                }
            }
        }

        public bool AllowDrag
        {
            get
            {
                switch (Type)
                {
                    case NodeType.FOLDER:
                        return !IsNameEditable;

                    case NodeType.PRODUCT:
                        return true;

                    case NodeType.INVENTORY:
                        return false;

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
                    case NodeType.FOLDER:
                        return true;

                    case NodeType.PRODUCT:
                        return false;

                    case NodeType.INVENTORY:
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
                    case NodeType.FOLDER:
                        return true;

                    case NodeType.PRODUCT:
                        return false;

                    case NodeType.INVENTORY:
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
                    case NodeType.FOLDER:
                        return Brushes.Tan;

                    case NodeType.PRODUCT:
                        return Brushes.DeepPink;

                    case NodeType.INVENTORY:
                        return Brushes.DarkGray;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// TextBlock 수정 여부
        /// </summary>
        public bool IsNameEditable
        {
            get
            {
                return _isNameEditable;
            }
            set
            {
                _isNameEditable = value;
                NotifyPropertyChanged("IsNameEditable");
            }
        }

        private Observable<Product> GetObservableProduct(string productID)
        {
            if (!string.IsNullOrEmpty(productID) && Type == NodeType.PRODUCT)
            {
                var ofd = InventoryDataCommander.GetInstance();
                Observable<Product> product = ofd.SearchObservableField<Product>(productID);
                return product;
            }
            return null;
        }

        private ObservableInventory GetObservableInventory(string inventoryFormatID)
        {
            if (!string.IsNullOrEmpty(inventoryFormatID) && Type == NodeType.INVENTORY)
            {
                var ofd = InventoryDataCommander.GetInstance();
                ObservableInventory inventory = ofd.SearchObservableInventory(inventoryFormatID);
                return inventory;
            }
            return null;
        }

        private void OnProductPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name" && Type == NodeType.PRODUCT)
            {
                Observable<Product> project = sender as Observable<Product>;
                Name = project.Name; //반드시 _name에 대입해야한다. Name에 대입할 경우 무한루프에 빠짐
            }
        }

        private void OnInventoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Specification" && Type == NodeType.INVENTORY)
            {
                ObservableInventory inventory = sender as ObservableInventory;
                Name = inventory.Specification; //반드시 _name에 대입해야한다. Name에 대입할 경우 무한루프에 빠짐
            }
        }

        protected void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}