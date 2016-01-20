using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace R54IN0.WPF
{
    public partial class IOStockDataAmenderViewModel
    {
       
        /// <summary>
        /// 제품 탐색기 뷰모델
        /// </summary>
        public MultiSelectTreeViewModelView TreeViewViewModel { get; set; }

        /// <summary>
        /// 제품 탐색기 열기 버튼의 명령어 
        /// </summary>
        public RelayCommand ProductSearchCommand { get; set; }

        /// <summary>
        /// 저장 버튼 명령어
        /// </summary>
        public RelayCommand RecordCommand { get; set; }

        /// <summary>
        /// 제품 탐색기에서 제품을 선택한 뒤, 확인 버튼의 Command 객체
        /// </summary>
        public RelayCommand ProductSelectCommand { get; set; }

        /// <summary>
        /// 최근 저장된 데이터를 불러온다.
        /// </summary>
        public RelayCommand LoadLastRecordCommand { get; set; }

        public RelayCommand<RoutedEventArgs> ProjectComboBoxGotFocusEventCommand { get; set; }

        public bool IsEditableSpecification
        {
            get
            {
                return _isEditableSpecification;
            }
            set
            {
                _isEditableSpecification = value;
                NotifyPropertyChanged("IsEditableSpecification");
            }
        }

        /// <summary>
        /// 제품 탐색기 플라이아웃의 가시 여부 바인딩 프로퍼티
        /// </summary>
        public bool IsOpenFlyout
        {
            get
            {
                return _isOpenFlyout;
            }
            set
            {
                _isOpenFlyout = value;
                NotifyPropertyChanged("IsOpenFlyout");
            }
        }

        /// <summary>
        /// 단가의 합계 (입출된 개수 * 가격)
        /// </summary>
        public decimal Amount
        {
            get
            {
                return Quantity * UnitPrice;
            }
        }

        /// <summary>
        /// 재고 수량
        /// Inventory.Quantity는 입출고 수량에 의한 변동된 재고수량을 계산하기 위해 가만히 둔다.
        /// </summary>
        public int InventoryQuantity
        {
            get
            {
                return _inventoryQuantity;
            }
            private set
            {
                _inventoryQuantity = value;
                NotifyPropertyChanged("InventoryQuantity");
            }
        }

        public string AccountTypeText
        {
            get
            {
                return _accountTypeText;
            }
            set
            {
                _accountTypeText = value;
                NotifyPropertyChanged("AccountTypeText");
            }
        }

        public string TitleText
        {
            get; set;
        }

        public Visibility LoadLastRecordVisibility
        {
            get;
            set;
        }

        #region IsEnabled Property

        public bool IsEnabledDatePicker
        {
            get
            {
                return _isEnabledDatePicker;
            }
            set
            {
                _isEnabledDatePicker = value;
                NotifyPropertyChanged("IsEnabledDatePicker");
            }
        }

        public bool IsEnabledWarehouseComboBox
        {
            get
            {
                return _isEnabledWarehouseComboBox;
            }
            set
            {
                _isEnabledWarehouseComboBox = value;
                NotifyPropertyChanged("IsEnabledWarehouseComboBox");
            }
        }

        public bool IsEnabledProjectComboBox
        {
            get
            {
                return _isEnabledProjectComboBox;
            }
            set
            {
                _isEnabledProjectComboBox = value;
                NotifyPropertyChanged("IsEnabledProjectComboBox");
            }
        }

        public bool IsEnabledInComingRadioButton
        {
            get
            {
                return _isEnabledInComingRadioButton;
            }
            set
            {
                _isEnabledInComingRadioButton = value;
                NotifyPropertyChanged("IsEnabledInComingRadioButton");
            }
        }

        public bool IsEnabledOutGoingRadioButton
        {
            get
            {
                return _isEnabledInOutGoingRadioButton;
            }
            set
            {
                _isEnabledInOutGoingRadioButton = value;
                NotifyPropertyChanged("IsEnabledOutGoingRadioButton");
            }
        }

        public bool IsEnabledSpecificationComboBox
        {
            get
            {
                return _isEnabledSpecificationComboBox;
            }
            set
            {
                _isEnabledSpecificationComboBox = value;
                NotifyPropertyChanged("IsEnabledSpecificationComboBox");
            }
        }

        #endregion IsEnabled Property

        #region IsReadOnly Property

        /// <summary>
        /// 제품 텍스트 박스의 텍스트 바인딩 프로퍼티
        /// </summary>
        public bool IsReadOnlyProductTextBox
        {
            get
            {
                return _isReadOnlyProductTextBox;
            }
            set
            {
                _isReadOnlyProductTextBox = value;
                NotifyPropertyChanged("IsReadOnlyProductTextBox");
            }
        }
        #endregion IsReadOnly Property
    }
}