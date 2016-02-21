﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;

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

        ///// <summary>
        ///// 체크된 모든 입출고데이터를 오늘날짜로 복사하기
        ///// </summary>
        public RelayCommand CheckedIOStockFormatsCopyCommand { get; set; }

        ///// <summary>
        ///// 체크된 모든 입출고 데이터를 삭제하기
        ///// </summary>
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
                MainWindowViewModel.GetInstance().ShowInventoryStatus(SelectedItem.Inventory.ID);
        }

        /// <summary>
        /// 제품별 입출고 데이터로 보기
        /// </summary>
        private void ExecuteSearchAsIOStockRecordCommand()
        {
            if (SelectedItem != null)
                MainWindowViewModel.GetInstance().ShowIOStockStatus(SelectedItem.Inventory.ID);
        }

        /// <summary>
        /// 새로운 입출고 데이터 추가하기
        /// </summary>
        private void ExecuteNewIOStockFormatAdditionCommand()
        {
            MainWindowViewModel main = MainWindowViewModel.GetInstance();
            if (SelectedItem != null)
                main.IOStockViewModel.OpenManager(SelectedItem.Inventory as ObservableInventory);
        }

        /// <summary>
        /// 기존의 입출고 데이터 수정하기
        /// </summary>
        private void ExecuteIOStockFormatModificationCommand()
        {
            if (SelectedItem != null)
            {
                MainWindowViewModel main = MainWindowViewModel.GetInstance();
                main.IOStockViewModel.OpenManager(SelectedItem);
            }
        }

        /// <summary>
        /// 선택된 입출고 데이터 삭제하기
        /// </summary>
        private void ExecuteIOStockFormatDeletionCommand()
        {
            if (SelectedItem != null)
            {
                var item = SelectedItem;
                CollectionViewModelObserverSubject.GetInstance().NotifyItemDeleted(item);
                DataDirector.GetInstance().DB.Delete<IOStockFormat>(item.ID);
                SelectedItem = null;
            }
        }

        /// <summary>
        /// 체크된 입출고 데이터 삭제하기
        /// </summary>
        private void ExecuteChekcedIOStockFormatsDeletionCommand()
        {
            var items = Items.Where(x => x.IsChecked == true).ToList();
            foreach (var item in items)
            {
                CollectionViewModelObserverSubject.GetInstance().NotifyItemDeleted(item);
                DataDirector.GetInstance().DB.Delete<IOStockFormat>(item.ID);
            }
        }

        /// <summary>
        /// 체크된 입출고 데이터를 오늘날짜로 복사하여 데이터그리드에 추가하기
        /// </summary>
        private void ExecuteCheckedIOStockFormatsCopyCommand()
        {
            var items = Items.Where(x => x.IsChecked == true).ToList();
            foreach (var item in items)
            {
                var copyFmt = new IOStockFormat(item.Format) { ID = null };
                IOStockDataGridItem copy = new IOStockDataGridItem(copyFmt);
                copy.Date = DateTime.Now;
                CollectionViewModelObserverSubject.GetInstance().NotifyNewItemAdded(copy);
            }
            var invens = items.Select(x => x.Inventory).Distinct();
        }
    }
}