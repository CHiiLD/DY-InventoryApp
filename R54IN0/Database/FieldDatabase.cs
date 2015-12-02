using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace R54IN0
{
    public class FieldDatabase : IDisposable
    {
        public WFieldSortedObservableCollection<WItem, Item> SortedItemList{ get; private set; }
        public WFieldSortedObservableCollection<WSpecification, Specification> SortedSpecList{ get; private set; }
        public WFieldSortedObservableCollection<WAccount, Account> SortedAccountList{ get; private set; }
        public WFieldSortedObservableCollection<WCurrency, Currency> SortedCurrencyList{ get; private set; }
        public WFieldSortedObservableCollection<WEmployee, Employee> SortedEmployeeList{ get; private set; }
        public WFieldSortedObservableCollection<WMaker, Maker> SortedMakerList{ get; private set; }
        public WFieldSortedObservableCollection<WMeasure, Measure> SortedMeasureList{ get; private set; }
        public WFieldSortedObservableCollection<WWarehouse, Warehouse> SortedWarehouseList{ get; private set; }

        public WFieldSortedObservableCollection<WItem, Item> EnableSortedItemList { get; private set; }
        public WFieldSortedObservableCollection<WSpecification, Specification> EnableSortedSpecList { get; private set; }
        public WFieldSortedObservableCollection<WAccount, Account> EnableSortedAccountList { get; private set; }
        public WFieldSortedObservableCollection<WCurrency, Currency> EnableSortedCurrencyList { get; private set; }
        public WFieldSortedObservableCollection<WEmployee, Employee> EnableSortedEmployeeList { get; private set; }
        public WFieldSortedObservableCollection<WMaker, Maker> EnableSortedMakerList { get; private set; }
        public WFieldSortedObservableCollection<WMeasure, Measure> EnableSortedMeasureList { get; private set; }
        public WFieldSortedObservableCollection<WWarehouse, Warehouse> EnableSortedWarehouseList { get; private set; }

        void InitSortedDictionary()
        {
            SortedItemList = new WFieldSortedObservableCollection<WItem, Item>();
            SortedSpecList = new WFieldSortedObservableCollection<WSpecification, Specification>();
            SortedAccountList = new WFieldSortedObservableCollection<WAccount, Account>();
            SortedCurrencyList = new WFieldSortedObservableCollection<WCurrency, Currency>();
            SortedEmployeeList = new WFieldSortedObservableCollection<WEmployee, Employee>();
            SortedMakerList = new WFieldSortedObservableCollection<WMaker, Maker>();
            SortedMeasureList = new WFieldSortedObservableCollection<WMeasure, Measure>();
            SortedWarehouseList = new WFieldSortedObservableCollection<WWarehouse, Warehouse>();

            using (var db = DatabaseDirector.GetDbInstance())
            {
                Account[] accounts = db.LoadAll<Account>();
                foreach (var i in accounts)
                    SortedAccountList.Add(new WAccount(i));

                Currency[] currency = db.LoadAll<Currency>();
                foreach (var i in currency)
                    SortedCurrencyList.Add(new WCurrency(i));

                Employee[] eeployee = db.LoadAll<Employee>();
                foreach (var i in eeployee)
                    SortedEmployeeList.Add(new WEmployee(i));

                Maker[] maker = db.LoadAll<Maker>();
                foreach (var i in maker)
                    SortedMakerList.Add(new WMaker(i));

                Measure[] measure = db.LoadAll<Measure>();
                foreach (var i in measure)
                    SortedMeasureList.Add(new WMeasure(i));

                Warehouse[] warehouse = db.LoadAll<Warehouse>();
                foreach (var i in warehouse)
                    SortedWarehouseList.Add(new WWarehouse(i));

                Item[] item = db.LoadAll<Item>();
                foreach (var i in item)
                    SortedItemList.Add(new WItem(i));

                Specification[] spec = db.LoadAll<Specification>();
                foreach (var i in spec)
                    SortedSpecList.Add(new WSpecification(i));
            }

            EnableSortedAccountList = new WFieldSortedObservableCollection<WAccount, Account>(SortedAccountList.Where(x => !x.IsDeleted));
            EnableSortedCurrencyList = new WFieldSortedObservableCollection<WCurrency, Currency>(SortedCurrencyList.Where(x => !x.IsDeleted));
            EnableSortedEmployeeList = new WFieldSortedObservableCollection<WEmployee, Employee>(SortedEmployeeList.Where(x => !x.IsDeleted));
            EnableSortedMakerList = new WFieldSortedObservableCollection<WMaker, Maker>(SortedMakerList.Where(x => !x.IsDeleted));
            EnableSortedMeasureList = new WFieldSortedObservableCollection<WMeasure, Measure>(SortedMeasureList.Where(x => !x.IsDeleted));
            EnableSortedWarehouseList = new WFieldSortedObservableCollection<WWarehouse, Warehouse>(SortedWarehouseList.Where(x => !x.IsDeleted));

            EnableSortedItemList = new WFieldSortedObservableCollection<WItem, Item>(SortedItemList.Where(x => !x.IsDeleted));
            EnableSortedSpecList = new WFieldSortedObservableCollection<WSpecification, Specification>(SortedSpecList.Where(x => !x.IsDeleted));
        }

        public FieldDatabase()
        {
            InitSortedDictionary();
        }

        public void Dispose()
        {

        }
    }
}