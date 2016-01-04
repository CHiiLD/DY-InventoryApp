using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R54IN0.WPF
{
    public class InventoryStatusViewModel
    {
        public InventoryDataGridViewModel DataGridViewModel1 { get; set; }
        public InventoryDataGridViewModel DataGridViewModel2 { get; set; }
        public InventorySearchTextBoxViewModel SearchViewModel { get; set; }

        public InventoryStatusViewModel()
        {
            Initialize();
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

            List<ObservableInventory> list = ObservableInvenDirector.GetInstance().CreateList();
            PushDataGridItems(list, true);
        }

        /// <summary>
        /// 왼쪽 데이터 그리드의 아이템이 선택될 경우 오른쪽 데이터 그리드의 아이템의 상태를 Null로 전환하고
        /// 오른쪽 데이터 그리드의 아이템이 선택될 경우 왼쪽 데이터 그리드의 아이템의 상태를 Null로 전환한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == DataGridViewModel1 && DataGridViewModel1.SelectedItem != null)
                DataGridViewModel2.SelectedItem = null;
            else if (sender == DataGridViewModel2 && DataGridViewModel2.SelectedItem != null)
                DataGridViewModel1.SelectedItem = null;
        }

        /// <summary>
        /// 검색 명령어 실행
        /// </summary>
        /// <param name="parameter"></param>
        protected void ExecuteSearchCommand(object parameter)
        {
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
            if (doClear)
            {
                DataGridViewModel1.Items.Clear();
                DataGridViewModel2.Items.Clear();
            }
            foreach (var item in items)
            {
                if (DataGridViewModel1.Items.Count <= DataGridViewModel2.Items.Count)
                    DataGridViewModel1.Items.Add(item);
                else
                    DataGridViewModel2.Items.Add(item);
            }
            DataGridViewModel1.SelectedItem = null;
            DataGridViewModel2.SelectedItem = null;
        }
    }
}
