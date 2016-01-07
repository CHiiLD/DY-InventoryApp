using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace R54IN0.WPF
{
    public class InoutStockStatusViewModel : INotifyPropertyChanged
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

        Visibility _datePickerViewModelVisibility;
        Visibility _projectListBoxViewModelVisibility;
        Visibility _treeViewViewModelVisibility;

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

        public InoutStockStatusViewModel()
        {
            DataGridViewModel = new InoutStockDataGridVIewModel();
            ProjectListBoxViewModel = new InoutStockProjectListBoxViewModel();
            TreeViewViewModel = new ProductManagerViewModel();
            DatePickerViewModel = new InoutStockDatePickerViewModel();

            IsCheckedInComing = true;
            IsCheckedOutGoing = true;
        }

        public InoutStockDataGridVIewModel DataGridViewModel
        {
            get; set;
        }

        public InoutStockProjectListBoxViewModel ProjectListBoxViewModel
        {
            get;
            set;
        }

        public ProductManagerViewModel TreeViewViewModel
        {
            get;
            set;
        }

        public InoutStockDatePickerViewModel DatePickerViewModel
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
                NotifyPropertyChanged("IsCheckedOutGoing");
            }
        }

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

        public void NotifyPropertyChanged(string name)
        {
            if (_propertyChanged != null)
                _propertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}