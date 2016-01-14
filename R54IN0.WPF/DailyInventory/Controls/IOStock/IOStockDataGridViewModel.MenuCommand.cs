using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;

namespace R54IN0.WPF
{
    public partial class IOStockDataGridViewModel
    {
        /// <summary>
        /// 제품별 입출고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsIOStockRecordCommand { get; set; }

        /// <summary>
        /// 입출고 데이터를 새로 추가하기
        /// </summary>
        public RelayCommand NewIOStockFormatAdditionCommand { get; set; }

        /// <summary>
        /// 선택된 입출고 데이터를 수정하기
        /// </summary>
        public RelayCommand IOStockFormatModificationCommand { get; set; }

        /// <summary>
        /// 선택된 입출고 데이터를 삭제하기
        /// </summary>
        public RelayCommand IOStockFormatDeletionCommand { get; set; }

        /// <summary>
        /// 체크된 모든 입출고데이터를 오늘날짜로 복사하기
        /// </summary>
        public RelayCommand CheckedIOStockFormatsCopyCommand { get; set; }

        /// <summary>
        /// 체크된 모든 입출고 데이터를 삭제하기
        /// </summary>
        public RelayCommand ChekcedIOStockFormatsDeletionCommand { get; set; }

        /// <summary>
        /// 재고 현황으로 보기
        /// </summary>
        public RelayCommand SearchAsInventoryRecordCommand { get; set; }

        public bool IsSelected()
        {
            return SelectedItem != null;
        }

        /// <summary>
        /// 재고현황으로 데이터 보기
        /// </summary>
        private void ExecuteSearchAsInventoryRecordCommand()
        {
            if (SelectedItem != null)
                MainWindowViewModel.GetInstance().ShowInventoryStatus(SelectedItem.Inventory.Product.ID);
        }

        /// <summary>
        /// 제품별 입출고 데이터로 보기
        /// </summary>
        private void ExecuteSearchAsIOStockRecordCommand()
        {
            if (SelectedItem != null)
                MainWindowViewModel.GetInstance().ShowIOStockStatusByProduct(SelectedItem.Inventory.Product.ID);
        }

        /// <summary>
        /// 새로운 입출고 데이터 추가하기
        /// </summary>
        private void ExecuteNewIOStockFormatAdditionCommand()
        {
            MainWindowViewModel main = MainWindowViewModel.GetInstance();
            if (SelectedItem != null)
                main.IOStockViewModel.OpenIOStockDataAmenderWindow(SelectedItem.Inventory);
            else
                main.IOStockViewModel.OpenIOStockDataAmenderWindow();
        }

        /// <summary>
        /// 기존의 입출고 데이터 수정하기
        /// </summary>
        private void ExecuteIOStockFormatModificationCommand()
        {
            if (SelectedItem != null)
            {
                MainWindowViewModel main = MainWindowViewModel.GetInstance();
                main.IOStockViewModel.OpenIOStockDataAmenderWindow(SelectedItem);
            }
        }

        /// <summary>
        /// 선택된 입출고 데이터 삭제하기
        /// </summary>
        private void ExecuteIOStockFormatDeletionCommand()
        {
        }

        /// <summary>
        /// 체크된 입출고 데이터 삭제하기
        /// </summary>
        private void ExecuteChekcedIOStockFormatsDeletionCommand()
        {
        }

        /// <summary>
        /// 체크된 입출고 데이터를 오늘날짜로 복사하여 데이터그리드에 추가하기
        /// </summary>
        private void ExecuteCheckedIOStockFormatsCopyCommand()
        {
            List<IOStockDataGridItem> list = new List<IOStockDataGridItem>();
            foreach (var item in Items)
            {
                //TODO 이제 추가된 새로운 데이터들의 수량만큼 재고수량과 잔여수량을 적용시켜야함 그리고 삭제 기능들을 넣자..
                if (item.IsChecked == true)
                {
                    IOStockDataGridItem copy = new IOStockDataGridItem(item);
                    copy.IsChecked = false;
                    copy.Date = DateTime.Now;
                    list.Add(copy);
                }
            }
            foreach (var item in list)
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(item);
        }
    }
}