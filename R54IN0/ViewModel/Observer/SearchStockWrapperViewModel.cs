using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace R54IN0
{
    public class SearchStockWrapperViewModel : StockWrapperViewModel, IFieldSearchingWithDateTime, IFinderViewModelCreatation
    {
        DateTime _fromDateTime;
        DateTime _toDateTime;

        public SearchStockWrapperViewModel(StockType type, CollectionViewModelObserverSubject subject) : base(type, subject)
        {
            SearchTypes = new Type[] { typeof(Item), typeof(Specification), typeof(Maker), typeof(Warehouse) };
            SelectedSearchType = SearchTypes.First();
            TodayCommand = new CommandHandler(ExecuteTodaySetCommand, ReturnTrue);
            YesterdayCommand = new CommandHandler(ExecuteYesterdaySetCommand, ReturnTrue);
            ThisWorkCommand = new CommandHandler(ExecuteThisWorkSetCommand, ReturnTrue);
            ThisMonthCommand = new CommandHandler(ExecuteThisMonthSetCommand, ReturnTrue);
            SearchCommand = new CommandHandler(ExecuteSearchCommand, ReturnTrue);

            ExecuteThisMonthSetCommand(null);
        }

        public CommandHandler TodayCommand { get; set; }

        public CommandHandler YesterdayCommand { get; set; }

        public CommandHandler ThisWorkCommand { get; set; }

        public CommandHandler ThisMonthCommand { get; set; }

        public CommandHandler SearchCommand { get; set; }

        public DateTime FromDateTime
        {
            get
            {
                return _fromDateTime;
            }
            set
            {
                _fromDateTime = value;
                OnPropertyChanged("FromDateTime");
            }
        }

        public DateTime ToDateTime
        {
            get
            {
                return _toDateTime;
            }
            set
            {
                _toDateTime = value;
                OnPropertyChanged("ToDateTime");
            }
        }

        public IEnumerable<Type> SearchTypes { get; set; }

        public Type SelectedSearchType { get; set; }

        public string Keyword
        {
            get;
            set;
        }

        public FinderViewModel FinderViewModel
        {
            get; set;
        }

        public override void UpdateNewItem(object item)
        {
            if (!(item is StockWrapper))
                return;
            var ioStockw = item as StockWrapper;
            if (!StockType.HasFlag(ioStockw.StockType))
                return;
            //모든 목록인 경우 
            if (FinderViewModel.SelectedNodes.Count() == 0 && stockDirector.Count(StockType) == Items.Count())
            {
                if (!Items.Contains(ioStockw)) //base.UpdateNewItem를 사용할 수 없어서 더 상위의 base.UpdateNewItem메서드 내용을 씀
                    Items.Add(ioStockw);
                return;
            }
            //Finder에 목록이 클릭되어 있는 경우
            var itemNodes = FinderViewModel.SelectedNodes.SelectMany(rn => rn.Descendants().Where(x => x.Type == NodeType.ITEM));
            if (itemNodes.Any(n => n.ItemUUID == ioStockw.Item.UUID))
            {
                if (!Items.Contains(ioStockw))
                    Items.Add(ioStockw);
            }
        }

        List<StockWrapper> Search<T>(string keyword)
        {
            string[] keywords = null;
            if (!string.IsNullOrEmpty(keyword))
                keywords = keyword.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            else
                keywords = new string[] { "" };
            var fwd = FieldWrapperDirector.GetInstance();
            var swd = StockWrapperDirector.GetInstance();
            var iwd = InventoryWrapperDirector.GetInstance();
            List<StockWrapper> stockwList = new List<StockWrapper>();
            IEnumerable<ItemWrapper> itemws = null;
            IEnumerable<InventoryWrapper> invenws = null;

            var itemwColl = fwd.CreateCollection<Item, ItemWrapper>();
            if (typeof(T) == typeof(Item))
                itemws = keywords.SelectMany(word => itemwColl.Where(item => item.Name.Contains(word)));
            else if (typeof(T) == typeof(Maker))
                itemws = keywords.SelectMany(word => itemwColl.Where(item => item.SelectedMaker != null).Where(item => item.SelectedMaker.Name.Contains(word)));
            else if (typeof(T) == typeof(Warehouse))
                invenws = keywords.SelectMany(word => iwd.CreateCollection().Where(x => x.Warehouse != null).Where(x => x.Warehouse.Name.Contains(word)));
            else if (typeof(T) == typeof(Specification))
                invenws = keywords.SelectMany(word => iwd.CreateCollection().Where(x => x.Specification.Name.Contains(word)));
            else
                throw new ArgumentException();

            IEnumerable<StockWrapper> stockws = null;
            if (itemws != null)
                stockws = itemws.SelectMany(itemw => swd.SearchAsItemkey(itemw.UUID));
            else if (invenws != null)
                stockws = invenws.SelectMany(invenw => swd.SearchAsInventoryKey(invenw.UUID));
            else
                return null;
            foreach (var stockw in stockws)
            {
                if (stockw != null)
                    stockwList.Add(stockw);
            }
            return stockwList;
        }

        public void SearchKeyword<T>(string keyword)
        {
            var result = Search<T>(keyword);
            Items = result != null ? new ObservableCollection<StockWrapper>(result) : null;
        }

        public void SearchKeyword<T>(string keyword, DateTime from, DateTime to)
        {
            ObservableCollection<StockWrapper> items = new ObservableCollection<StockWrapper>();
            List<StockWrapper> stockws = Search<T>(keyword);
            if (stockws == null)
                Items = null;

            //처음부터 끝까지 Date를 비교연산하여 찾기 -> O(N) 
            //Date 정렬 + Date 찾기 -> O(logN) + O(N) = O(N)
            MultiSortedDictionary<DateTime, StockWrapper> dic = new MultiSortedDictionary<DateTime, StockWrapper>();
            foreach (var stockw in stockws)
                dic.Add(stockw.Date, stockw);

            foreach (var dateTime in dic.keys)
            {
                if (dateTime < from)
                    continue;
                if (dateTime > to)
                    break;
                foreach (var item in dic[dateTime])
                    items.Add(item);
            }
            Items = items;
        }

        public void SearchKeyword<T>()
        {
            if (Keyword == null)
                Keyword = "";
            var from = new DateTime(FromDateTime.Year, FromDateTime.Month, FromDateTime.Day, 0, 0, 0, 0, DateTimeKind.Local);
            var to = new DateTime(ToDateTime.Year, ToDateTime.Month, ToDateTime.Day, 23, 59, 59, 999, DateTimeKind.Local);
            SearchKeyword<T>(Keyword, from, to);
            if (FinderViewModel != null) //Finder에선 선택되는 아이템이 없다.
            {
                FinderViewModel.SelectedNodes.Clear(); // = null 을 하게 되면 에러 발생
                FinderViewModel.OnPropertyChanged("SelectedNodes");
            }
        }

        void ExecuteSearchCommand(object obj)
        {
            SelectedItem = null;
            Items = null;
            if (SelectedSearchType == typeof(Item))
                SearchKeyword<Item>();
            else if (SelectedSearchType == typeof(Specification))
                SearchKeyword<Specification>();
            else if (SelectedSearchType == typeof(Warehouse))
                SearchKeyword<Warehouse>();
            else if (SelectedSearchType == typeof(Maker))
                SearchKeyword<Maker>();
            else
                throw new Exception();
        }

        void ExecuteThisMonthSetCommand(object obj)
        {
            var now = DateTime.Now;
            FromDateTime = new DateTime(now.Year, now.Month, 1, 0, 0, 0, 0, DateTimeKind.Local);
            ToDateTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        void ExecuteThisWorkSetCommand(object obj)
        {
            var now = DateTime.Now;
            var work = now.AddDays(-(int)now.DayOfWeek);
            FromDateTime = new DateTime(work.Year, work.Month, work.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDateTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        void ExecuteYesterdaySetCommand(object obj)
        {
            var yesterday = DateTime.Now.AddDays(-1);
            FromDateTime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDateTime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        bool ReturnTrue(object arg)
        {
            return true;
        }

        void ExecuteTodaySetCommand(object obj)
        {
            var now = DateTime.Now;
            FromDateTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0, DateTimeKind.Local);
            ToDateTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, 999, DateTimeKind.Local);
        }

        public FinderViewModel CreateFinderViewModel(TreeViewEx treeView)
        {
            FinderViewModel = new ItemFinderViewModel(treeView);
            FinderViewModel.SelectItemsChanged += OnFinderViewSelectItemChanged;
            return FinderViewModel;
        }
    }
}