using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace R54IN0.WPF
{
    public class InventoryStatusViewModel : INotifyPropertyChanged, ICollectionViewModelObserver
    {
        private bool _canModify;
        private bool? _showProductColumn;
        private bool? _showMakerColumn;
        private bool? _showMeasureColumn;

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

        public InventoryStatusViewModel()
        {
            Initialize();
            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        ~InventoryStatusViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        public InventoryDataGridViewModel DataGridViewModel1 { get; set; }

        public InventoryDataGridViewModel DataGridViewModel2 { get; set; }

        public InventorySearchTextBoxViewModel SearchViewModel { get; set; }

        public ProductSelectorViewModel TreeViewViewModel { get; set; }

        /// <summary>
        /// ToggleSwitch 데이터그리드의 IsReadOnly프로퍼티와 연결
        /// </summary>
        public bool CanModify
        {
            get
            {
                return _canModify;
            }
            set
            {
                _canModify = value;
                DataGridViewModel1.IsReadOnly = !value;
                DataGridViewModel2.IsReadOnly = !value;
                NotifyPropertyChanged("CanModify");
            }
        }

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowProductColumn
        {
            get
            {
                return _showProductColumn;
            }
            set
            {
                _showProductColumn = value;
                DataGridViewModel1.ProductVisibility = DataGridViewModel2.ProductVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowProductColumn");
            }
        }

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowMakerColumn
        {
            get
            {
                return _showMakerColumn;
            }
            set
            {
                _showMakerColumn = value;
                DataGridViewModel1.MakerVisibility = DataGridViewModel2.MakerVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowMakerColumn");
            }
        }

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowMeasureColumn
        {
            get
            {
                return _showMeasureColumn;
            }
            set
            {
                _showMeasureColumn = value;
                DataGridViewModel1.MeasureVisibility = DataGridViewModel2.MeasureVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowMeasureColumn");
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        protected virtual void Initialize()
        {
            DataGridViewModel1 = new InventoryDataGridViewModel();
            DataGridViewModel2 = new InventoryDataGridViewModel();

            DataGridViewModel1.PropertyChanged += OnDataGridViewPropertyChanged;
            DataGridViewModel2.PropertyChanged += OnDataGridViewPropertyChanged;

            SearchViewModel = new InventorySearchTextBoxViewModel();
            SearchViewModel.SearchCommand = new CommandHandler(ExecuteSearchCommand, (object obj) => { return true; });

            List<ObservableInventory> list = ObservableInventoryDirector.GetInstance().CreateList();
            PushDataGridItems(list, true);

            CanModify = false;
            ShowProductColumn = ShowMeasureColumn = ShowMakerColumn = true;

            TreeViewViewModel = new ProductSelectorViewModel();
            TreeViewViewModel.PropertyChanged += OnTreeViewPropertyChanged;
        }

        /// <summary>
        /// SelectedNodes가 선택될 때 DataGrid를 업데이트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNodes" && sender == TreeViewViewModel)
            {
                var proNodes = TreeViewViewModel.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT));
                var oid = ObservableInventoryDirector.GetInstance();
                var invenList = oid.CreateList();

                List<ObservableInventory> temp = new List<ObservableInventory>();
                foreach (var node in proNodes)
                {
                    var invens = invenList.Where(inven => inven.Product.ID == node.ProductID).OrderBy(inven => inven.Specification);
                    temp.AddRange(invens);
                }
                PushDataGridItems(temp, true);
                DataGridViewModel1.SelectedItem = null;
                DataGridViewModel2.SelectedItem = null;
            }
        }

        /// <summary>
        /// 왼쪽 데이터 그리드의 아이템이 선택될 경우 오른쪽 데이터 그리드의 아이템의 상태를 Null로 전환하고
        /// 오른쪽 데이터 그리드의 아이템이 선택될 경우 왼쪽 데이터 그리드의 아이템의 상태를 Null로 전환한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                if (sender == DataGridViewModel1 && DataGridViewModel1.SelectedItem != null)
                    DataGridViewModel2.SelectedItem = null;
                else if (sender == DataGridViewModel2 && DataGridViewModel2.SelectedItem != null)
                    DataGridViewModel1.SelectedItem = null;
            }
        }

        /// <summary>
        /// 검색 명령어 실행
        /// </summary>
        /// <param name="parameter"></param>
        protected void ExecuteSearchCommand(object parameter)
        {
            TreeViewViewModel.SelectedNodes.Clear();
            var searchResult = SearchViewModel.Search();
            PushDataGridItems(searchResult, true);
        }

        /// <summary>
        /// ObservableInventory 객체들을 왼, 오른쪽 데이터 그리드에 각각 배치한다.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="doClear">기존의 데이터를 지우고 데이터를 추가할 경우 true, 아니면 false</param>
        protected void PushDataGridItems(IEnumerable<ObservableInventory> items, bool doClear = false)
        {
            if (items == null)
                return;
            //프로젝트 이름순으로 정렬하기
            MultiSortedDictionary<string, ObservableInventory> multiSortedDic = new MultiSortedDictionary<string, ObservableInventory>();
            foreach (var item in items)
                multiSortedDic.Add(item.Product.Name, item);
            ObservableCollection<ObservableInventory> collection = null;
            int half = items.Count() / 2;
            if (doClear) //기존 데이터 삭제
            {
                DataGridViewModel1.Items.Clear();
                DataGridViewModel2.Items.Clear();
                DataGridViewModel1.SelectedItem = null;
                DataGridViewModel2.SelectedItem = null;
            }
            foreach (string key in multiSortedDic.keys)
            {
                if (doClear)
                {
                    collection = DataGridViewModel1.Items.Count() < half ? DataGridViewModel1.Items : DataGridViewModel2.Items;
                }
                else
                {
                    if (DataGridViewModel1.Items.Any(item => item.Product.Name == key)) //왼쪽 데이터그리드에 동일한 제품 이름을 가진 경우
                        collection = DataGridViewModel1.Items;
                    else if (DataGridViewModel2.Items.Any(item => item.Product.Name == key)) //오른쪽 데이터그리드에 동일한 제품 이름을 가진 경우
                        collection = DataGridViewModel2.Items;
                    else //동일한 제품이 없는 경우 균형을 맞추기 위해 데이터가 적은 데이터그리드부터 채운다.
                        collection = DataGridViewModel1.Items.Count() <= DataGridViewModel2.Items.Count() ? DataGridViewModel1.Items : DataGridViewModel2.Items;
                }
                foreach (ObservableInventory inventory in multiSortedDic[key].OrderBy(item => item.Specification))
                    collection.Add(inventory);
            }
        }

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// LR 데이터 그리드의 아이템들을 합해서 가져온다.
        /// </summary>
        /// <returns></returns>
        public List<ObservableInventory> GetDataGridItems()
        {
            var list = new List<ObservableInventory>();
            list.AddRange(DataGridViewModel1.Items);
            list.AddRange(DataGridViewModel2.Items);
            return list;
        }

        public void UpdateNewItem(object item)
        {
            if (item is ObservableInventory)
            {
                var observableInventory = item as ObservableInventory;
                var nodes = TreeViewViewModel.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT));
                if (TreeViewViewModel.SelectedNodes.Count == 0 || nodes.Any(x => x.ProductID == observableInventory.Product.ID))
                    PushDataGridItems(new ObservableInventory[] { observableInventory });
            }
        }

        public void UpdateDelItem(object item)
        {
        }
    }
}