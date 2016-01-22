using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace R54IN0.WPF
{
    public class InventorySearchTextBoxViewModel
    {
        /// <summary>
        /// TextBox Search Button Command Binding
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// TextBox binding
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 검색 실행
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ObservableInventory> Search()
        {
            var inventories = InventoryDataCommander.GetInstance().CopyObservableInventories();
            if (string.IsNullOrEmpty(Text))
                return inventories;
            string[] keywords = Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var result = keywords.SelectMany(word => inventories.Where(inven => inven.Specification.Contains(word) || inven.Product.Name.Contains(word)));
            return result;
        }
    }

    public class FilterSearchTextBoxViewModel : InventorySearchTextBoxViewModel, ICollectionViewModel<string>
    {
        public const string FILTER_PRODUCT = "제품";
        public const string FILTER_SPECIFICATION = "규격";
        public const string FILTER_MAKER = "제조사";
        public const string FILTER_SUPPLIER = "구입처";
        public const string FILTER_WAREHOUSE = "보관장소";
        public const string FILTER_CUSTOMER = "출고처";
        public const string FILTER_EMPLOYEE = "담당자";

        public FilterSearchTextBoxViewModel()
        {
            Items = new ObservableCollection<string>()
            {
                FILTER_PRODUCT,
                FILTER_SPECIFICATION,
                FILTER_MAKER,
                FILTER_SUPPLIER,
                FILTER_WAREHOUSE,
                FILTER_CUSTOMER,
                FILTER_EMPLOYEE,
            };
            SelectedItem = Items.First();
        }

        public ObservableCollection<string> Items
        {
            get; set;
        }

        public string SelectedItem
        {
            get; set;
        }

        public async Task<IEnumerable<IOStockFormat>> SearchAsFilter()
        {
            string[] keywords = Text.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            List<IOStockFormat> result = new List<IOStockFormat>();
            string column = string.Empty;
            if (SelectedItem == FILTER_PRODUCT || SelectedItem == FILTER_SPECIFICATION || SelectedItem == FILTER_MAKER)
            {
                IEnumerable<ObservableInventory> inventories = InventoryDataCommander.GetInstance().CopyObservableInventories();
                IEnumerable<ObservableInventory> match = null;
                switch (SelectedItem)
                {
                    case FILTER_PRODUCT:
                        match = keywords.SelectMany(word => inventories.Where(inven => inven.Product.Name.Contains(word)));
                        break;
                    case FILTER_SPECIFICATION:
                        match = keywords.SelectMany(word => inventories.Where(inven => inven.Specification.Contains(word)));
                        break;
                    case FILTER_MAKER:
                        match = keywords.SelectMany(word => inventories.Where(inven => inven.Maker != null && inven.Maker.Name.Contains(word)));
                        break;
                }
                foreach (var inventory in match)
                    result.AddRange(await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(DbCommand.WHERE, "InventoryID", inventory.ID));
            }
            else if (SelectedItem == FILTER_SUPPLIER || SelectedItem == FILTER_WAREHOUSE || SelectedItem == FILTER_CUSTOMER || SelectedItem == FILTER_EMPLOYEE)
            {
                IEnumerable<IField> fields = null;
                switch (SelectedItem)
                {
                    case FILTER_SUPPLIER:
                        fields = await DbAdapter.GetInstance().SelectAllAsync<Supplier>();
                        column = "SupplierID";
                        break;
                    case FILTER_WAREHOUSE:
                        fields = await DbAdapter.GetInstance().SelectAllAsync<Warehouse>();
                        column = "WarehouseID";
                        break;
                    case FILTER_CUSTOMER:
                        fields = await DbAdapter.GetInstance().SelectAllAsync<Customer>();
                        column = "CustomerID";
                        break;
                    case FILTER_EMPLOYEE:
                        fields = await DbAdapter.GetInstance().SelectAllAsync<Employee>();
                        column = "EmployeeID";
                        break;
                }
                IEnumerable<IField> match = keywords.SelectMany(word => fields.Where(x => x.Name != null && x.Name.Contains(word)));
                Debug.WriteLine("- 검색 결과 이름이 일치된 필드 이름들");
                foreach (var field in match)
                    Debug.WriteLine(field.Name);
                foreach (var field in match)
                {
                    var fmts = await DbAdapter.GetInstance().QueryAsync<IOStockFormat>(DbCommand.WHERE, column, field.ID);
                    result.AddRange(fmts);
                }
            }
            return result;
        }
    }
}