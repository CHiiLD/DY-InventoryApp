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

        private string[] _groupItems = new string[] { GROUPITEM_DATE, GROUPITEM_PROJECT, GROUPITEM_PRODUCT };
        private string _selectedGroupItem;

        private string[] _userHelperTexts = new string[] { "Date Picker", "프로젝트 리스트", "제품 탐색기" };

        private bool? _isCheckedInComing;
        private bool? _isCheckedOutGoing;

        private string _dataGridHelperHeader;

        private Visibility _datePickerViewModelVisibility;
        private Visibility _projectListBoxViewModelVisibility;
        private Visibility _treeViewViewModelVisibility;

        /// <summary>
        /// 데이터 그리드의 입출고 데이터를 일시적으로 보관
        /// </summary>
        private IEnumerable<IOStockDataGridItem> _backupSource;

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

        public IOStockDataGridViewModel DataGridViewModel
        {
            get; set;
        }

        public IOStockProjectListBoxViewModel ProjectListBoxViewModel
        {
            get;
            set;
        }

        public ProductSelectorViewModel TreeViewViewModel
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
            TreeViewViewModel = new ProductSelectorViewModel();
            DatePickerViewModel = new IOStockDatePickerViewModel();

            IsCheckedInComing = true;
            IsCheckedOutGoing = true;

            DatePickerViewModel.CommandExecuted += OndatePickerCommandExecuted;
            ProjectListBoxViewModel.PropertyChanged += OnProjectListPropertyChanged;
            TreeViewViewModel.PropertyChanged += OnTreeViewNodesSelected;

            NewInoutStockAddCommand = new CommandHandler(ExecuteNewInoutStockAddCommand, (object obj) => { return true; });
            DataGridViewModel.PropertyChanged += OnDataGridViewModelPropertyChanged;
            SelectedItemModifyCommand = new CommandHandler(ExecuteSelectedItemModifyCommand, CanModifySelectedItem);
        }

        private bool CanModifySelectedItem(object arg)
        {
            return DataGridViewModel.SelectedItem != null ? true : false;
        }

        private void ExecuteSelectedItemModifyCommand(object obj)
        {
            OpenAmender(DataGridViewModel.SelectedItem);
        }

        private void OpenAmender(IObservableIOStockProperties selectedItem = null)
        {
            var win = new IOStockDataAmenderWindow();
            IOStockDataAmenderViewModel amenderViewModel = null;
            if (selectedItem != null)
                amenderViewModel = new IOStockDataAmenderViewModel(selectedItem);
            else
                amenderViewModel = new IOStockDataAmenderViewModel();
            win.DataContext = amenderViewModel;
            win.ProductSelector.DataContext = amenderViewModel.TreeViewViewModel;
            win.ProductSelector.MultiSelectTreeView.TreeView.OnSelecting += amenderViewModel.TreeViewViewModel.OnNodeSelected;
            win.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            win.ShowDialog();
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
                var cmd = SelectedItemModifyCommand as CommandHandler;
                cmd.UpdateCanExecute();
            }
        }

        /// <summary>
        /// 새로운 입출고 데이터를 생성하기 위해서 InoutStockRegisterWindow를 다이얼로그로 호출한다.
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteNewInoutStockAddCommand(object obj)
        {
            OpenAmender();
        }

        /// <summary>
        /// 트리뷰에서 제품들을 선택했을 경우, 이와 관련된 입출고 데이터를 보여준다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTreeViewNodesSelected(object sender, PropertyChangedEventArgs e)
        {
            if (sender == TreeViewViewModel && e.PropertyName == "SelectedNodes")
            {
                DataGridViewModel.Items.Clear();
                var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));
                var oid = ObservableInventoryDirector.GetInstance();
                List<ObservableInventory> observableInventoryList = new List<ObservableInventory>();
                List<IOStockFormat> inoutStockFormatList = new List<IOStockFormat>();
                foreach (var node in nodes)
                {
                    var searchResult = oid.SearchAsProductID(node.ProductID);
                    if (searchResult != null)
                        observableInventoryList.AddRange(searchResult);
                }
                foreach (var inven in observableInventoryList)
                {
                    IIndexQuery<IOStockFormat, string> formats = null;
                    using (var db = LexDb.GetDbInstance())
                        formats = db.Table<IOStockFormat>().IndexQueryByKey("InventoryID", inven.ID);
                    if (formats != null)
                        inoutStockFormatList.AddRange(formats.ToList());
                }
                _backupSource = inoutStockFormatList.Select(x => new IOStockDataGridItem(x));
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
                    IIndexQuery<IOStockFormat, string> formats = null;
                    using (var db = LexDb.GetDbInstance())
                        formats = db.Table<IOStockFormat>().IndexQueryByKey("ProjectID", proejct.ID);
                    if (formats != null)
                    {
                        _backupSource = formats.ToList().Select(x => new IOStockDataGridItem(x));
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
        private void OndatePickerCommandExecuted(object sender, EventArgs e)
        {
            if (sender == DatePickerViewModel)
            {
                DataGridViewModel.Items.Clear();
                var fromDate = DatePickerViewModel.FromDate;
                var toDate = DatePickerViewModel.ToDate;
                IOStockFormat[] formats = null;
                using (var db = LexDb.GetDbInstance())
                    formats = db.Table<IOStockFormat>().LoadAll();
                if (formats != null)
                {
                    _backupSource = formats.Where(fmt => fromDate <= fmt.Date && fmt.Date <= toDate).Select(x => new IOStockDataGridItem(x));
                    UpdateDataGridItems();
                }
            }
        }

        /// <summary>
        /// 입고, 출고에 따라 백업 데이터를 사용하여 데이터그리드의 아이템을 초기화한다.
        /// </summary>
        private void UpdateDataGridItems()
        {
            if (_backupSource == null)
                return;

            IOStockType type = IOStockType.NONE;
            if (IsCheckedInComing == true)
                type = type | IOStockType.INCOMING;
            if (IsCheckedOutGoing == true)
                type = type | IOStockType.OUTGOING;

            var items = _backupSource.Where(x => type.HasFlag(x.StockType)).OrderBy(x => x.Date);
            DataGridViewModel.Items = new ObservableCollection<IOStockDataGridItem>(items);
        }

        public void UpdateNewItem(object item)
        {
            if (item is ObservableIOStock)
            {
                if (_backupSource == null)
                    return;
                var observableInoutStock = item as ObservableIOStock;
                bool can = false;
                switch (SelectedGroupItem)
                {
                    case GROUPITEM_DATE:
                        DateTime fromDate = DatePickerViewModel.FromDate;
                        DateTime toDate = DatePickerViewModel.ToDate;
                        if (fromDate <= observableInoutStock.Date && observableInoutStock.Date <= toDate)
                            can = true;
                        break;

                    case GROUPITEM_PROJECT:
                        if (ProjectListBoxViewModel.SelectedItem != null && ProjectListBoxViewModel.SelectedItem.ID == observableInoutStock.Project.ID)
                            can = true;
                        break;

                    case GROUPITEM_PRODUCT:
                        var nodes = TreeViewViewModel.SelectedNodes.SelectMany(c => c.Descendants().Where(node => node.Type == NodeType.PRODUCT));
                        if (nodes.Any(node => node.ProductID == observableInoutStock.Inventory.Product.ID))
                            can = true;
                        break;
                }
                if (can)
                {
                    List<IOStockDataGridItem> list = null;
                    if (_backupSource is List<IOStockDataGridItem>)
                        list = _backupSource as List<IOStockDataGridItem>;
                    else
                        list = _backupSource.ToList();
                    list.Add(new IOStockDataGridItem(observableInoutStock.Format));
                    _backupSource = list;
                    UpdateDataGridItems();
                }
            }
        }

        public void UpdateDelItem(object item)
        {
        }
    }
}