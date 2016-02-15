using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class IOStockStatusViewModel : INotifyPropertyChanged, ICollectionViewModelObserver
    {
        public const string GROUPITEM_DATE = "날짜별";
        public const string GROUPITEM_PROJECT = "프로젝트별";
        public const string GROUPITEM_PRODUCT = "제품별";

        private string[] _groupItems = new string[] { GROUPITEM_PRODUCT, GROUPITEM_DATE, GROUPITEM_PROJECT };
        private string _selectedGroupItem;

        private string[] _userHelperTexts = new string[] { "제품 탐색기", "Date Picker", "프로젝트 리스트" };

        private bool? _isCheckedInComing;
        private bool? _isCheckedOutGoing;

        private string _dataGridHelperHeader;

        private Visibility _datePickerViewModelVisibility;
        private Visibility _projectListBoxViewModelVisibility;
        private Visibility _treeViewViewModelVisibility;

        private bool _canModify;
        private bool? _showSpecificationMemoColumn;
        private bool? _showMakerColumn;
        private bool? _showRemainQtyColumn;
        private bool? _showSecondStockTypeColumn;

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

        public IOStockStatusViewModel()
        {
            Initialize();
            CollectionViewModelObserverSubject.GetInstance().Attach(this);
        }

        ~IOStockStatusViewModel()
        {
            CollectionViewModelObserverSubject.GetInstance().Detach(this);
        }

        public List<IOStockDataGridItem> DataGridItemsSource
        {
            get;
            set;
        }

        #region ViewModel

        /// <summary>
        /// 데이터 그리드 뷰모델
        /// </summary>
        public IOStockDataGridViewModel DataGridViewModel { get; private set; }

        /// <summary>
        /// 프로젝트 리스트 뷰모델
        /// </summary>
        public IOStockProjectListBoxViewModel ProjectListBoxViewModel { get; private set; }

        /// <summary>
        /// 트리뷰 뷰모델
        /// </summary>
        public MultiSelectTreeViewModelView TreeViewViewModel { get; private set; }

        /// <summary>
        /// 데이터 선택하기 뷰모델
        /// </summary>
        public IOStockDatePickerViewModel DatePickerViewModel { get; private set; }

        /// <summary>
        /// 검색기능 뷰모델
        /// </summary>
        public FilterSearchTextBoxViewModel SearchViewModel { get; set; }

        #endregion ViewModel

        /// <summary>
        /// 그룹박스 헤더 바인딩
        /// </summary>
        public string DataGridHelperHeader
        {
            get
            {
                return _dataGridHelperHeader;
            }
            set
            {
                _dataGridHelperHeader = value;
                NotifyPropertyChanged("DataGridHelperHeader");
            }
        }

        /// <summary>
        /// 그룹화 리스트박스 아이템들
        /// </summary>
        public string[] GroupItems
        {
            get
            {
                return _groupItems;
            }
        }

        /// <summary>
        /// 그룹화 리스트 박스의 선택된 아이템
        /// </summary>
        public string SelectedGroupItem
        {
            get
            {
                return _selectedGroupItem;
            }
            set
            {
                _selectedGroupItem = value;
                int i = 0;
                for (; i < GroupItems.Count(); i++)
                {
                    if (_selectedGroupItem == GroupItems[i])
                    {
                        DataGridHelperHeader = _userHelperTexts[i];
                        break;
                    }
                }
                DatePickerViewModelVisibility = Visibility.Collapsed;
                TreeViewViewModelVisibility = Visibility.Collapsed;
                ProjectListBoxViewModelVisibility = Visibility.Collapsed;

                if (_selectedGroupItem == GROUPITEM_DATE)
                    DatePickerViewModelVisibility = Visibility.Visible;
                else if (_selectedGroupItem == GROUPITEM_PRODUCT)
                    TreeViewViewModelVisibility = Visibility.Visible;
                else if (_selectedGroupItem == GROUPITEM_PROJECT)
                    ProjectListBoxViewModelVisibility = Visibility.Visible;

                NotifyPropertyChanged("SelectedGroupItem");
            }
        }

        /// <summary>
        /// 입고 체크 바인딩
        /// </summary>
        public bool? IsCheckedInComing
        {
            get
            {
                return _isCheckedInComing;
            }
            set
            {
                _isCheckedInComing = value;
                UpdateDataGridItems();
                NotifyPropertyChanged("IsCheckedInComing");
            }
        }

        /// <summary>
        /// 출고 체크 바인딩
        /// </summary>
        public bool? IsCheckedOutGoing
        {
            get
            {
                return _isCheckedOutGoing;
            }
            set
            {
                _isCheckedOutGoing = value;
                UpdateDataGridItems();
                NotifyPropertyChanged("IsCheckedOutGoing");
            }
        }

        #region ViewModel Visibility

        /// <summary>
        /// 제품 탐색기 가시 여부
        /// </summary>
        public Visibility TreeViewViewModelVisibility
        {
            get
            {
                return _treeViewViewModelVisibility;
            }
            set
            {
                _treeViewViewModelVisibility = value;
                NotifyPropertyChanged("TreeViewViewModelVisibility");
            }
        }

        /// <summary>
        /// 날짜 선택기 가시 여부
        /// </summary>
        public Visibility DatePickerViewModelVisibility
        {
            get
            {
                return _datePickerViewModelVisibility;
            }
            set
            {
                _datePickerViewModelVisibility = value;
                NotifyPropertyChanged("DatePickerViewModelVisibility");
            }
        }

        /// <summary>
        /// 프로젝트 리스트 가시 여부
        /// </summary>
        public Visibility ProjectListBoxViewModelVisibility
        {
            get
            {
                return _projectListBoxViewModelVisibility;
            }
            set
            {
                _projectListBoxViewModelVisibility = value;
                NotifyPropertyChanged("ProjectListBoxViewModelVisibility");
            }
        }

        #endregion ViewModel Visibility

        /// <summary>
        /// 새로운 입출고 기록을 추가하는 명령어
        /// </summary>
        public ICommand NewInoutStockAddCommand { get; set; }

        /// <summary>
        /// 선택된 셀을 수정하는 명령어
        /// </summary>
        public ICommand SelectedItemModifyCommand { get; set; }

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
                DataGridViewModel.IsReadOnly = !value;
                NotifyPropertyChanged("CanModify");
            }
        }

        #region Column Visibility

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowSpecificationMemoColumn
        {
            get
            {
                return _showSpecificationMemoColumn;
            }
            set
            {
                _showSpecificationMemoColumn = value;
                DataGridViewModel.SpecificationMemoColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowSpecificationMemoColumn");
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
                DataGridViewModel.MakerColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowMakerColumn");
            }
        }

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowRemainQtyColumn
        {
            get
            {
                return _showRemainQtyColumn;
            }
            set
            {
                _showRemainQtyColumn = value;
                DataGridViewModel.RemainQtyColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowRemainQtyColumn");
            }
        }

        /// <summary>
        /// CheckBox IsChecked 바인딩 프로퍼티
        /// </summary>
        public bool? ShowSecondStockTypeColumn
        {
            get
            {
                return _showSecondStockTypeColumn;
            }
            set
            {
                _showSecondStockTypeColumn = value;
                DataGridViewModel.SecondStockTypeColumnVisibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                NotifyPropertyChanged("ShowSecondStockTypeColumn");
            }
        }

        #endregion Column Visibility

        public void NotifyPropertyChanged(string name)

        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// 초기화
        /// </summary>
        protected virtual void Initialize()
        {
            DataGridViewModel = new IOStockDataGridViewModel();
            ProjectListBoxViewModel = new IOStockProjectListBoxViewModel();
            TreeViewViewModel = new MultiSelectTreeViewModelView();
            DatePickerViewModel = new IOStockDatePickerViewModel();
            SearchViewModel = new FilterSearchTextBoxViewModel();
            SearchViewModel.SearchCommand = new RelayCommand(ExecuteSearchCommand, CanSearch);

            IsCheckedInComing = true;
            IsCheckedOutGoing = true;

            DatePickerViewModel.CommandExecuted += OnDatePickerCommandExecuted;
            ProjectListBoxViewModel.PropertyChanged += OnProjectListPropertyChanged;
            TreeViewViewModel.PropertyChanged += OnTreeViewNodesSelected;

            NewInoutStockAddCommand = new RelayCommand<object>(ExecuteNewInoutStockAddCommand, (object obj) => { return true; });
            DataGridViewModel.PropertyChanged += OnDataGridViewModelPropertyChanged;
            SelectedItemModifyCommand = new RelayCommand<object>(ExecuteSelectedItemModifyCommand, CanModifySelectedItem);

            ShowMakerColumn = true;
            ShowRemainQtyColumn = true;
            ShowSecondStockTypeColumn = true;
            ShowSpecificationMemoColumn = true;
            CanModify = false;

            SelectedGroupItem = GroupItems.First();

            DataDirector.GetInstance().DB.DataInsertEventHandler += DataInserted;
            DataDirector.GetInstance().DB.DataDeleteEventHandler += DataDeleted;
        }
        private bool CanModifySelectedItem(object arg)
        {
            return DataGridViewModel.SelectedItem != null ? true : false;
        }

        private void ExecuteSelectedItemModifyCommand(object obj)
        {
            OpenIOStockDataAmenderWindow(DataGridViewModel.SelectedItem);
        }

        public void OpenIOStockDataAmenderWindow(IObservableIOStockProperties selectedItem = null)
        {
            IOStockDataAmenderViewModel viewmodel = null;
            if (selectedItem != null)
                viewmodel = new IOStockDataAmenderViewModel(selectedItem);
            else
                viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.TreeViewViewModel.DragCommand = null;
            viewmodel.TreeViewViewModel.DropCommand = null;
            IOStockDataAmenderWindow window = new IOStockDataAmenderWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void OpenIOStockDataAmenderWindow(Observable<Product> product)
        {
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.TreeViewViewModel.DragCommand = null;
            viewmodel.TreeViewViewModel.DropCommand = null;
            viewmodel.Product = product;
            IOStockDataAmenderWindow window = new IOStockDataAmenderWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void OpenIOStockDataAmenderWindow(IObservableInventoryProperties inventory)
        {
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel();
            viewmodel.TreeViewViewModel.DragCommand = null;
            viewmodel.TreeViewViewModel.DropCommand = null;
            viewmodel.Product = inventory.Product;
            if (inventory != null)
                viewmodel.Inventory = viewmodel.InventoryList.Where(x => x.ID == inventory.ID).SingleOrDefault();
            IOStockDataAmenderWindow window = new IOStockDataAmenderWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        /// <summary>
        /// 데이터그리드 뷰모델에서 PropertyChange 핸들러가 호출된 경우
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == DataGridViewModel && e.PropertyName == "SelectedItem")
            {
                var cmd = SelectedItemModifyCommand as RelayCommand<object>;
                cmd.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 새로운 입출고 데이터를 생성하기 위해서 InoutStockRegisterWindow를 다이얼로그로 호출한다.
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteNewInoutStockAddCommand(object obj)
        {
            OpenIOStockDataAmenderWindow();
        }

        /// <summary>
        /// 트리뷰에서 제품들을 선택했을 경우, 이와 관련된 입출고 데이터를 보여준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnTreeViewNodesSelected(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
            {
                DataGridViewModel.Items.Clear();
                var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));

                var inode = TreeViewViewModel.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.INVENTORY));
                var pnode = TreeViewViewModel.SelectedNodes.SelectMany(x => x.Descendants().Where(y => y.Type == NodeType.PRODUCT));
                pnode = pnode.SelectMany(x => x.Root.Select(y => y));
                var unionnode = inode.Union(pnode);

                List<IOStockFormat> format = new List<IOStockFormat>();
                var inventories = unionnode.Select(x => DataDirector.GetInstance().SearchInventory(x.ObservableObjectID));
                foreach (var inventory in inventories)
                {
                    IEnumerable<IOStockFormat> fmt = DataDirector.GetInstance().DB.Query<IOStockFormat>(
                        "select * from {0} where {1} = '{2}';", typeof(IOStockFormat).Name, "InventoryID", inventory.ID);
                    if (fmt != null)
                        format.AddRange(fmt);
                }
                DataGridItemsSource = format.Select(x => new IOStockDataGridItem(x)).ToList();
                UpdateDataGridItems();
            }
        }

        /// <summary>
        /// 프로젝트 리스트에서 프로젝트를 선택한 경우, 프로젝트와 관련된 모든 입출고 데이터를 보여준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnProjectListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == ProjectListBoxViewModel && e.PropertyName == "SelectedItem")
            {
                DataGridViewModel.Items.Clear();
                Observable<Project> proejct = ProjectListBoxViewModel.SelectedItem;
                if (proejct != null)
                {
                    IEnumerable<IOStockFormat> formats = DataDirector.GetInstance().DB.Query<IOStockFormat>(
                        "select * from {0} where {1} = '{2}';",
                        typeof(IOStockFormat).Name, "ProjectID", proejct.ID);
                    if (formats != null)
                    {
                        DataGridItemsSource = formats.Select(x => new IOStockDataGridItem(x)).ToList();
                        UpdateDataGridItems();
                    }
                }
            }
        }

        /// <summary>
        /// 날짜 선택란에서 Command를 실행한 경우, 해당 날짜 범위에 기록된 입출고 데이터를 보여준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDatePickerCommandExecuted(object sender, EventArgs e)
        {
            if (sender == DatePickerViewModel)
            {
                DataGridViewModel.Items.Clear();
                var fromDate = DatePickerViewModel.FromDate;
                var toDate = DatePickerViewModel.ToDate;
                string datetimeFmt = SQLiteClient.DATETIME;
                var formats = DataDirector.GetInstance().DB.Query<IOStockFormat>(
                    "select * from {0} where {1} between '{2}' and '{3}';", typeof(IOStockFormat).Name, "Date", fromDate.ToString(datetimeFmt), toDate.ToString(datetimeFmt));
                if (formats != null)
                {
                    DataGridItemsSource = formats.Select(x => new IOStockDataGridItem(x)).ToList();
                    UpdateDataGridItems();
                }
            }
        }

        /// <summary>
        /// 입고, 출고에 따라 백업 데이터를 사용하여 데이터그리드의 아이템을 초기화한다.
        /// </summary>
        private void UpdateDataGridItems()
        {
            IOStockType type = IOStockType.NONE;
            if (IsCheckedInComing == true)
                type = type | IOStockType.INCOMING;
            if (IsCheckedOutGoing == true)
                type = type | IOStockType.OUTGOING;

            var items = DataGridItemsSource.Where(x => type.HasFlag(x.StockType)).OrderBy(x => x.Date);
            DataGridViewModel.Items = new ObservableCollection<IOStockDataGridItem>(items);
        }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(SearchViewModel.Text);
        }

        /// <summary>
        /// 검색 명령어 실행
        /// </summary>
        /// <param name="parameter"></param>
        protected void ExecuteSearchCommand()
        {
            TreeViewViewModel.SelectedNodes.Clear();
            DataGridViewModel.Items.Clear();
            IEnumerable<IOStockFormat> fmts = SearchViewModel.SearchAsFilter();
            DataGridItemsSource = fmts.Select(x => new IOStockDataGridItem(x)).ToList();
            UpdateDataGridItems();
        }

        public void UpdateNewItem(object item)
        {

        }

        public void UpdateDelItem(object item)
        {
            if (item is Observable<Customer>)
                DataGridItemsSource.ForEach(x => { if (x.Customer == item) x.Customer = null; });
            else if (item is Observable<Supplier>)
                DataGridItemsSource.ForEach(x => { if (x.Supplier == item) x.Supplier = null; });
            else if (item is Observable<Project>)
                DataGridItemsSource.ForEach(x => { if (x.Project == item) x.Project = null; });
            else if (item is Observable<Warehouse>)
                DataGridItemsSource.ForEach(x => { if (x.Warehouse == item) x.Warehouse = null; });
            else if (item is Observable<Employee>)
                DataGridItemsSource.ForEach(x => { if (x.Employee == item) x.Employee = null; });
        }

        private void DataInserted(object sender, SQLInsDelEventArgs e)
        {
            var data = e.Data;
            if (data is IOStockFormat)
            {
                var fmt = data as IOStockFormat;
                var item = new IOStockDataGridItem(fmt);
                bool canAdd = false;
                switch (SelectedGroupItem)
                {
                    case GROUPITEM_DATE:
                        DateTime fromDate = DatePickerViewModel.FromDate;
                        DateTime toDate = DatePickerViewModel.ToDate;
                        if (fromDate <= item.Date && item.Date <= toDate)
                            canAdd = true;
                        break;
                    case GROUPITEM_PROJECT:
                        if (ProjectListBoxViewModel.SelectedItem != null && ProjectListBoxViewModel.SelectedItem.ID == item.Project.ID)
                            canAdd = true;
                        break;
                    case GROUPITEM_PRODUCT:
                        var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));
                        if (nodes.Any(node => node.ObservableObjectID == item.Inventory.Product.ID))
                            canAdd = true;
                        break;
                }
                if (canAdd)
                {
                    DataGridItemsSource.Add(item);
                    UpdateDataGridItems();
                }
            }
        }

        private void DataDeleted(object sender, SQLInsDelEventArgs e)
        {
            if (DataGridItemsSource == null)
                return;
            var data = e.Data;
            if (data is IOStockFormat)
            {
                var fmt = data as IOStockFormat;
                var i = DataGridItemsSource.Where(x => x.ID == fmt.ID).SingleOrDefault();
                if(i != null)
                    DataGridItemsSource.Remove(i);
            }
        }
    }
}