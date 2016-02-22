using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace R54IN0.WPF
{
    public class IOStockStatusViewModel : INotifyPropertyChanged, ICollectionViewModelObserver
    {
        public const string DATAGRID_OPTION_DATE = "날짜별";
        public const string DATAGRID_OPTION_PROJECT = "프로젝트별";
        public const string DATAGRID_OPTION_PRODUCT = "제품별";

        private string[] _dataGridGroupOptions = new string[] { DATAGRID_OPTION_PRODUCT, DATAGRID_OPTION_DATE, DATAGRID_OPTION_PROJECT };
        private string _selectedGroupoption;

        private string[] _userHelperTexts = new string[] { "제품 탐색기", "Date Picker", "프로젝트 리스트" };

        private bool _isCheckedInComing;
        private bool _isCheckedOutGoing;

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
        public string DataGridOptionGroupBoxHeader
        {
            get
            {
                return _dataGridHelperHeader;
            }
            set
            {
                _dataGridHelperHeader = value;
                NotifyPropertyChanged("DataGridOptionGroupBoxHeader");
            }
        }

        /// <summary>
        /// 그룹화 리스트박스 아이템들
        /// </summary>
        public string[] DataGridGroupOptions
        {
            get
            {
                return _dataGridGroupOptions;
            }
        }

        /// <summary>
        /// 그룹화 리스트 박스의 선택된 아이템
        /// </summary>
        public string SelectedDataGridGroupOption
        {
            get
            {
                return _selectedGroupoption;
            }
            set
            {
                _selectedGroupoption = value;
                int i = 0;
                for (; i < DataGridGroupOptions.Count(); i++)
                {
                    if (_selectedGroupoption == DataGridGroupOptions[i])
                    {
                        DataGridOptionGroupBoxHeader = _userHelperTexts[i];
                        break;
                    }
                }
                if (_selectedGroupoption == DATAGRID_OPTION_DATE)
                {
                    DatePickerViewModelVisibility = Visibility.Visible;
                    TreeViewViewModelVisibility = Visibility.Collapsed;
                    ProjectListBoxViewModelVisibility = Visibility.Collapsed;
                    DataGridViewModel.RemainQtyColumnVisibility = Visibility.Collapsed;
                }
                else if (_selectedGroupoption == DATAGRID_OPTION_PRODUCT)
                {
                    DatePickerViewModelVisibility = Visibility.Collapsed;
                    ProjectListBoxViewModelVisibility = Visibility.Collapsed;
                    TreeViewViewModelVisibility = Visibility.Visible;
                    DataGridViewModel.RemainQtyColumnVisibility = Visibility.Visible;
                    TreeViewViewModel.SelectedNodes.Clear();
                }
                else if (_selectedGroupoption == DATAGRID_OPTION_PROJECT)
                {
                    DatePickerViewModelVisibility = Visibility.Collapsed;
                    TreeViewViewModelVisibility = Visibility.Collapsed;
                    ProjectListBoxViewModelVisibility = Visibility.Visible;
                    DataGridViewModel.RemainQtyColumnVisibility = Visibility.Collapsed;
                    ProjectListBoxViewModel.SelectedItem = null;
                }
                else
                {
                    DatePickerViewModelVisibility = Visibility.Collapsed;
                    TreeViewViewModelVisibility = Visibility.Collapsed;
                    ProjectListBoxViewModelVisibility = Visibility.Collapsed;
                    DataGridViewModel.RemainQtyColumnVisibility = Visibility.Collapsed;
                }
                DataDirector.GetInstance().StockList.Clear();
                DataGridViewModel.Items.Clear();
                NotifyPropertyChanged("SelectedDataGridGroupOption");
            }
        }

        /// <summary>
        /// 입고 체크 바인딩
        /// </summary>
        public bool IsCheckedInComing
        {
            get
            {
                return _isCheckedInComing;
            }
            set
            {
                _isCheckedInComing = value;
                SetDataGridItems();
                NotifyPropertyChanged("IsCheckedInComing");
            }
        }

        /// <summary>
        /// 출고 체크 바인딩
        /// </summary>
        public bool IsCheckedOutGoing
        {
            get
            {
                return _isCheckedOutGoing;
            }
            set
            {
                _isCheckedOutGoing = value;
                SetDataGridItems();
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
        /// 선택된 셀을 수정하는 명령어
        /// </summary>
        public RelayCommand<object> SelectedItemModifyCommand { get; set; }

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

            DataGridViewModel.PropertyChanged += OnDataGridViewModelPropertyChanged;
            SelectedItemModifyCommand = new RelayCommand<object>(ExecuteSelectedItemModifyCommand, CanModifySelectedItem);

            ShowMakerColumn = true;
            ShowRemainQtyColumn = true;
            ShowSecondStockTypeColumn = true;
            ShowSpecificationMemoColumn = true;
            CanModify = false;

            SelectedDataGridGroupOption = DataGridGroupOptions.First();
        }

        private bool CanModifySelectedItem(object arg)
        {
            return DataGridViewModel.SelectedItem != null ? true : false;
        }

        /// <summary>
        /// IOStockFormat을 수정하기 위해
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteSelectedItemModifyCommand(object obj)
        {
            var item = DataGridViewModel.SelectedItem;
            OpenManager(item);
        }

        public void OpenManager(Observable<Product> product)
        {
            IOStockManagerViewModel viewmodel = new IOStockManagerViewModel(product);
            IOStockManagerWindow window = new IOStockManagerWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void OpenManager(ObservableInventory inventory)
        {
            IOStockManagerViewModel viewmodel = new IOStockManagerViewModel(inventory);
            IOStockManagerWindow window = new IOStockManagerWindow();
            window.DataContext = viewmodel;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        public void OpenManager(ObservableIOStock iostock)
        {
            IOStockManagerViewModel viewmodel = new IOStockManagerViewModel(this, iostock);
            IOStockManagerWindow window = new IOStockManagerWindow();
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
                SelectedItemModifyCommand.RaiseCanExecuteChanged();
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
                List<TreeViewNode> inode = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.INVENTORY);
                List<TreeViewNode> pnode = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.PRODUCT);
                IEnumerable<TreeViewNode> sumnode = pnode.SelectMany(x => x.Root.Select(y => y));
                IEnumerable<TreeViewNode> unionnode = inode.Union(sumnode);
                IEnumerable<ObservableInventory> invs = unionnode.Select(x => DataDirector.GetInstance().SearchInventory(x.ObservableObjectID));

                string sql = null;
                if (invs.Count() != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var inv in invs)
                    {
                        sb.Append('\'');
                        sb.Append(inv.ID);
                        sb.Append("', ");
                    }
                    if (sb.Length > 1)
                        sb.Remove(sb.Length - 2, 2);
                    sql = string.Format("select * from {0} where {1} in ({2}) order by Date desc;",
                        typeof(IOStockFormat).Name, "InventoryID", sb.ToString());
                }
                Dispatcher.CurrentDispatcher.BeginInvoke(new Func<string, Task>(SetDataGridItems), sql);
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
                Observable<Project> proejct = ProjectListBoxViewModel.SelectedItem;
                if (proejct != null)
                {
                    string sql = string.Format("select * from {0} where {1} = '{2}' order by Date desc;",
                        typeof(IOStockFormat).Name, "ProjectID", proejct.ID);
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Func<string, Task>(SetDataGridItems), sql);
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
                DateTime fromDate = DatePickerViewModel.FromDate;
                DateTime toDate = DatePickerViewModel.ToDate;
                string fmt = ClientAdapter.DATETIME;
                string sql = string.Format("select * from {0} where {1} between '{2}' and '{3}' order by Date desc;", 
                    typeof(IOStockFormat).Name, "Date", fromDate.ToString(fmt), toDate.ToString(fmt));
                Dispatcher.CurrentDispatcher.BeginInvoke(new Func<string, Task>(SetDataGridItems), sql);
            }
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
            SelectedDataGridGroupOption = null;
            string sql = SearchViewModel.SearchAsFilter();
            Dispatcher.CurrentDispatcher.BeginInvoke(new Func<string, Task>(SetDataGridItems), sql);
        }

        /// <summary>
        /// 입고, 출고에 따라 백업 데이터를 사용하여 데이터그리드의 아이템을 초기화한다.
        /// </summary>
        private async Task SetDataGridItems(string sql)
        {
            DataDirector.GetInstance().StockList.Clear();
            if (!string.IsNullOrEmpty(sql))
            {
                List<IOStockFormat> stofmts = await DataDirector.GetInstance().DB.QueryAsync<IOStockFormat>(sql);
                foreach (var stof in stofmts)
                {
                    IOStockDataGridItem item = new IOStockDataGridItem(stof);
                    DataDirector.GetInstance().StockList.Add(item);
                }
                CalcRemainQuantity();
            }
            SetDataGridItems();
        }

        public void SetDataGridItems()
        {
            DataGridViewModel.Items.Clear();
            IOStockType flag = GetCurrentStockTypeFlag();
            var coll = DataDirector.GetInstance().StockList.ToList();
            foreach (IOStockDataGridItem stock in coll)
            {
                if (!IsAddEnableInDataGridItems(stock))
                    DataDirector.GetInstance().StockList.Remove(stock);
                else if (flag.HasFlag(stock.StockType))
                    DataGridViewModel.Items.Add(stock);
            }
        }

        private IOStockType GetCurrentStockTypeFlag()
        {
            IOStockType flag = IOStockType.NONE;
            if (IsCheckedInComing)
                flag = flag | IOStockType.INCOMING;
            if (IsCheckedOutGoing)
                flag = flag | IOStockType.OUTGOING;
            return flag;
        }

        public void CalcRemainQuantity()
        {
            if (SelectedDataGridGroupOption != DATAGRID_OPTION_PRODUCT)
                return;

            List<IOStockDataGridItem> list = DataDirector.GetInstance().StockList;
            ILookup<string, IOStockDataGridItem> loopup = list.ToLookup(x => x.InventoryID);
            foreach (var l in loopup)
            {
                IEnumerable<IOStockDataGridItem> asc = l.OrderBy(x => x.Date);
                int qty;
                int? reQty = null;

                foreach (IOStockDataGridItem i in asc)
                {
                    qty = i.Quantity;
                    if (i.StockType == IOStockType.OUTGOING)
                        qty = -qty;
                    if (reQty == null)
                        reQty = 0;
                    reQty = i.RemainingQuantity = (int)reQty + qty;
                }
            }
        }

        /// <summary>
        /// DataGridViewModel.Items 컬렉션에 Add할 수 있는지 질의
        /// </summary>
        /// <param name="iStock"></param>
        /// <returns></returns>
        private bool IsAddEnableInDataGridItems(IObservableIOStockProperties iStock)
        {
            bool result = false;
            switch (SelectedDataGridGroupOption)
            {
                case DATAGRID_OPTION_DATE:
                    DateTime fromDate = DatePickerViewModel.FromDate;
                    DateTime toDate = DatePickerViewModel.ToDate;
                    if (fromDate <= iStock.Date && iStock.Date <= toDate)
                        result = true;
                    break;

                case DATAGRID_OPTION_PROJECT:
                    if (ProjectListBoxViewModel.SelectedItem != null)
                    {
                        if (iStock.Project != null && ProjectListBoxViewModel.SelectedItem.ID == iStock.Project.ID)
                            result = true;
                    }
                    break;

                case DATAGRID_OPTION_PRODUCT:
                    var nodes = TreeViewViewModel.SearchNodeInSelectedNodes(NodeType.PRODUCT | NodeType.INVENTORY);
                    if (nodes.Any(node => node.ObservableObjectID == iStock.Inventory.Product.ID || node.ObservableObjectID == iStock.Inventory.ID))
                        result = true;
                    break;

                default:
                    result = true;
                    break;
            }
            return result;
        }

        public void UpdateNewItem(object item)
        {
            if (item is IOStockDataGridItem)
            {
                IOStockDataGridItem stock = item as IOStockDataGridItem;
                if (IsAddEnableInDataGridItems(stock))
                {
                    DataDirector.GetInstance().StockList.Add(stock);

                    IOStockType flag = GetCurrentStockTypeFlag();
                    if (flag.HasFlag(stock.StockType))
                        DataGridViewModel.Items.Add(stock);

                    CalcRemainQuantity();
                }
            }
        }

        public void UpdateDelItem(object item)
        {
            if (item is IOStockDataGridItem)
            {
                IOStockDataGridItem stock = item as IOStockDataGridItem;

                DataDirector.GetInstance().StockList.Remove(stock);

                DataGridViewModel.Items.Remove(stock);

                CalcRemainQuantity();
            }
        }
    }
}