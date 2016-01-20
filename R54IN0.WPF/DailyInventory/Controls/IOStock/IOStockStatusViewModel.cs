using GalaSoft.MvvmLight.Command;
using Lex.Db;
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

        /// <summary>
        /// 데이터 그리드의 입출고 데이터를 일시적으로 보관
        /// </summary>
        private List<IOStockDataGridItem> _backupSource;

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

        public List<IOStockDataGridItem> BackupSource
        {
            get
            {
                return _backupSource;
            }
            set
            {
                _backupSource = value;
            }
        }

        public IOStockDataGridViewModel DataGridViewModel
        {
            get; set;
        }

        public IOStockProjectListBoxViewModel ProjectListBoxViewModel
        {
            get;
            set;
        }

        public MultiSelectTreeViewModelView TreeViewViewModel
        {
            get;
            set;
        }

        public IOStockDatePickerViewModel DatePickerViewModel
        {
            get;
            set;
        }

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

        public InventorySearchTextBoxViewModel SearchViewModel { get; set; }

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
            SearchViewModel = new InventorySearchTextBoxViewModel();
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
                viewmodel = new IOStockDataAmenderViewModel(this, selectedItem);
            else
                viewmodel = new IOStockDataAmenderViewModel(this);
            viewmodel.TreeViewViewModel.DragCommand = null;
            viewmodel.TreeViewViewModel.DropCommand = null;
            IOStockDataAmenderWindow window = new IOStockDataAmenderWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void OpenIOStockDataAmenderWindow(Observable<Product> product)
        {
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(this);
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
            IOStockDataAmenderViewModel viewmodel = new IOStockDataAmenderViewModel(this);
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
        public async void OnTreeViewNodesSelected(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
            {
                DataGridViewModel.Items.Clear();
                var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));
                var oid = ObservableInventoryDirector.GetInstance();
                List<ObservableInventory> obInvenList = new List<ObservableInventory>();
                List<IOStockFormat> iosFmtList = new List<IOStockFormat>();
                foreach (var node in nodes)
                {
                    var searchResult = oid.SearchAsProductID(node.ProductID);
                    if (searchResult != null)
                        obInvenList.AddRange(searchResult);
                }
                foreach (var inven in obInvenList)
                {
                    IEnumerable<IOStockFormat> formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(DbCommand.WHERE, "InventoryID", inven.ID);
                    if (formats != null)
                        iosFmtList.AddRange(formats);
                }
                BackupSource = iosFmtList.Select(x => new IOStockDataGridItem(x)).ToList();
                UpdateDataGridItems();
            }
        }

        /// <summary>
        /// 프로젝트 리스트에서 프로젝트를 선택한 경우, 프로젝트와 관련된 모든 입출고 데이터를 보여준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnProjectListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == ProjectListBoxViewModel && e.PropertyName == "SelectedItem")
            {
                DataGridViewModel.Items.Clear();
                Observable<Project> proejct = ProjectListBoxViewModel.SelectedItem;
                if (proejct != null)
                {
                    IEnumerable<IOStockFormat> formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(DbCommand.WHERE, "ProjectID", proejct.ID);
                    if (formats != null)
                    {
                        BackupSource = formats.Select(x => new IOStockDataGridItem(x)).ToList();
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
        private async void OnDatePickerCommandExecuted(object sender, EventArgs e)
        {
            if (sender == DatePickerViewModel)
            {
                DataGridViewModel.Items.Clear();
                var fromDate = DatePickerViewModel.FromDate;
                var toDate = DatePickerViewModel.ToDate;
                var formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(DbCommand.BETWEEN | DbCommand.OR_EQUAL, "Date", fromDate, toDate);
                if (formats != null)
                {
                    BackupSource = formats.Select(x => new IOStockDataGridItem(x)).ToList();
                    UpdateDataGridItems();
                }
            }
        }

        /// <summary>
        /// 입고, 출고에 따라 백업 데이터를 사용하여 데이터그리드의 아이템을 초기화한다.
        /// </summary>
        private void UpdateDataGridItems()
        {
            if (BackupSource == null)
                return;

            IOStockType type = IOStockType.NONE;
            if (IsCheckedInComing == true)
                type = type | IOStockType.INCOMING;
            if (IsCheckedOutGoing == true)
                type = type | IOStockType.OUTGOING;

            var items = BackupSource.Where(x => type.HasFlag(x.StockType)).OrderBy(x => x.Date);
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
        protected async void ExecuteSearchCommand()
        {
            TreeViewViewModel.SelectedNodes.Clear();
            DataGridViewModel.Items.Clear();
            BackupSource = new List<IOStockDataGridItem>();

            var searchResult = SearchViewModel.Search();
            foreach (var inven in searchResult)
            {
                var formats = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(
                    DbCommand.WHERE, "InventoryID", inven.ID);
                BackupSource.AddRange(formats.Select(x => new IOStockDataGridItem(x)));
            }
            UpdateDataGridItems();
        }

        public void UpdateNewItem(object item)
        {
            if (item is ObservableIOStock)
            {
                if (BackupSource == null)
                    return;
                var obIOStock = item as ObservableIOStock;
                bool can = false;
                switch (SelectedGroupItem)
                {
                    case GROUPITEM_DATE:
                        DateTime fromDate = DatePickerViewModel.FromDate;
                        DateTime toDate = DatePickerViewModel.ToDate;
                        if (fromDate <= obIOStock.Date && obIOStock.Date <= toDate)
                            can = true;
                        break;

                    case GROUPITEM_PROJECT:
                        if (ProjectListBoxViewModel.SelectedItem != null && ProjectListBoxViewModel.SelectedItem.ID == obIOStock.Project.ID)
                            can = true;
                        break;

                    case GROUPITEM_PRODUCT:
                        var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));
                        if (nodes.Any(node => node.ProductID == obIOStock.Inventory.Product.ID))
                            can = true;
                        break;
                }
                if (can)
                {
                    IOStockDataGridItem ioStockDataGridItem = new IOStockDataGridItem(obIOStock.Format);
                    BackupSource.Add(ioStockDataGridItem);
                    UpdateDataGridItems();
                }
            }
        }

        public void UpdateDelItem(object item)
        {
            IEnumerable<IOStockDataGridItem> items = null;
            if (item is Observable<Product>)
            {
                Observable<Product> product = item as Observable<Product>;
                if (BackupSource != null)
                    items = BackupSource.Where(x => x.Inventory.Product.ID == product.ID);
            }
            else if (item is IObservableInventoryProperties)
            {
                IObservableInventoryProperties obInven = item as IObservableInventoryProperties;
                if (BackupSource != null)
                    items = BackupSource.Where(x => x.Inventory.ID == obInven.ID);
            }
            else if (item is IObservableIOStockProperties)
            {
                IObservableIOStockProperties iostock = item as IObservableIOStockProperties;
                if (BackupSource != null)
                    items = BackupSource.Where(x => x.ID == iostock.ID);
            }

            if (items != null)
            {
                foreach (var i in items.ToList())
                {
                    BackupSource.Remove(i);
                    if (DataGridViewModel.Items.Contains(i))
                        DataGridViewModel.Items.Remove(i);
                }
            }
        }
    }
}