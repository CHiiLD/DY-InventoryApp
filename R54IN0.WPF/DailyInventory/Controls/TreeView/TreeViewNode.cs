using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace R54IN0.WPF
{
    public class TreeViewNode : INotifyPropertyChanged
    {
        private NodeType _type;
        private string _name;
        private bool _isNameEditable;
        private string _prodcutID;

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
            //ID = Guid.NewGuid().ToString();
            Root = new ObservableCollection<TreeViewNode>();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="type"></param>
        /// <param name="str">폴더인 경우 폴더이름을, 아이템인 경우 아이템 ID를 넣는다.</param>
        public TreeViewNode(NodeType type, string str) : this()
        {
            Type = type;
            if (Type == NodeType.FORDER)
                Name = str;
            else if (Type == NodeType.PRODUCT)
                ProductID = str;
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

        public string ProductID
        {
            get
            {
                return _prodcutID;
            }
            set
            {
                _prodcutID = value;
                Observable<Product> product = GetObservableProduct(value);
                if (product != null)
                {
                    Name = product.Name; //이름 적용
                    product.PropertyChanged += OnProductPropertyChanged; //이벤트 적용
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
                if (Type == NodeType.PRODUCT && IsNameEditable) //사용자가 입력을 끝냈을 때
                {
                    Observable<Product> product = GetObservableProduct(ProductID);
                    if (product != null)
                    {
                        product.PropertyChanged -= OnProductPropertyChanged;
                        product.Name = value;
                        product.PropertyChanged += OnProductPropertyChanged;
                    }
                }
                NotifyPropertyChanged("Name");
            }
        }

        public bool AllowDrag
        {
            get
            {
                switch (Type)
                {
                    case NodeType.FORDER:
                        return !IsNameEditable;

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

        private Observable<Product> GetObservableProduct(string id)
        {
            if (!string.IsNullOrEmpty(id) && Type == NodeType.PRODUCT)
            {
                var ofd = ObservableFieldDirector.GetInstance();
                Observable<Product> product = ofd.SearchObservableField<Product>(id);
                return product;
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

        protected void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}