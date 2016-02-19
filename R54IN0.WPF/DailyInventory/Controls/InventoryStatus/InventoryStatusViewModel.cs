using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
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

        #region ViewModel

        public InventoryDataGridViewModel DataGridViewModel1 { get; set; }

        public InventoryDataGridViewModel DataGridViewModel2 { get; set; }

        public InventorySearchTextBoxViewModel SearchViewModel { get; set; }

        public MultiSelectTreeViewModelView TreeViewViewModel { get; set; }

        #endregion ViewModel

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
                DataGridViewModel1.ProductColumnVisibility = DataGridViewModel2.ProductColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
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
                DataGridViewModel1.MakerColumnVisibility = DataGridViewModel2.MakerColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
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
                DataGridViewModel1.MeasureColumnVisibility = DataGridViewModel2.MeasureColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
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
            SearchViewModel.SearchCommand = new RelayCommand<object>(ExecuteSearchCommand, (object obj) => { return true; });

            List<ObservableInventory> list = DataDirector.GetInstance().CopyInventories();
            PushDataGridItems(list, true);

            CanModify = false;
            ShowProductColumn = ShowMeasureColumn = ShowMakerColumn = true;

            TreeViewViewModel = new MultiSelectTreeViewModelView();
            TreeViewViewModel.PropertyChanged += OnTreeViewPropertyChanged;
        }

        /// <summary>
        /// SelectedNodes가 선택될 때 DataGrid를 업데이트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedNodes" || sender != TreeViewViewModel)
                return;

            List<TreeViewNode> inode = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.INVENTORY);
            List<TreeViewNode> pnode = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.PRODUCT);
            IEnumerable<TreeViewNode> sumnode = pnode.SelectMany(x => x.Root.Select(y => y));
            var unionnode = inode.Union(sumnode);

            if (unionnode.Count() != 0)
            {
                var inventories = DataDirector.GetInstance().CopyInventories().Where(inven => unionnode.Any(n => n.ObservableObjectID == inven.ID));
                PushDataGridItems(inventories, true);
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
            if (doClear)
            {
                DataGridViewModel1.Items.Clear();
                DataGridViewModel2.Items.Clear();
                DataGridViewModel1.SelectedItem = null;
                DataGridViewModel2.SelectedItem = null;
            }
            var loopup = items.ToLookup(x => x.Product);
            var orderedItems = loopup.OrderBy(x => x.Key.Name);
            var dataGridItems1 = DataGridViewModel1.Items;
            var dataGridItems2 = DataGridViewModel2.Items;
            foreach (var key in orderedItems)
            {
                var orderedInvens = key.OrderBy(x => x.Specification);
                if (dataGridItems1.Count() == 0 || dataGridItems1.Count() <= dataGridItems2.Count())
                    orderedInvens.ToList().ForEach(x => dataGridItems1.Add(x));
                else
                    orderedInvens.ToList().ForEach(x => dataGridItems2.Add(x));
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
                var obInven = item as ObservableInventory;
                var nodes = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.PRODUCT);
                if (TreeViewViewModel.SelectedNodes.Count == 0 || nodes.Any(x => x.ObservableObjectID == obInven.Product.ID))
                    PushDataGridItems(new ObservableInventory[] { obInven });
            }
        }

        public void UpdateDelItem(object item)
        {
            if (item is ObservableInventory)
            {
                ObservableInventory inv = item as ObservableInventory;
                if (DataGridViewModel1.Items.Contains(inv))
                    DataGridViewModel1.Items.Remove(inv);
                else if (DataGridViewModel2.Items.Contains(inv))
                    DataGridViewModel2.Items.Remove(inv);
            }
            else if (item is Observable<Product>)
            {
                Observable<Product> product = item as Observable<Product>;
                foreach (var obInven in DataGridViewModel1.Items.ToList())
                {
                    if (obInven.Product.ID == product.ID)
                        DataGridViewModel1.Items.Remove(obInven);
                }
                foreach (var obInven in DataGridViewModel2.Items.ToList())
                {
                    if (obInven.Product.ID == product.ID)
                        DataGridViewModel2.Items.Remove(obInven);
                }
            }
        }
    }
}